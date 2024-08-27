using System.Text.RegularExpressions;
using System.Web;

namespace YouTubeStreamsExtractor
{
    public class Decryptor : IDecryptor
    {
        private readonly HttpClient _httpClient;
        private readonly ICache _cache;
        private IJavaScriptEngine _javaScriptEngine;

        public Decryptor(IJavaScriptEngine javaScriptEngine, HttpClient? httpClient = null, ICache? cache = null)
        {
            _javaScriptEngine = javaScriptEngine;

            if (httpClient == null)
            {
                _httpClient = new HttpClient();
            }
            else
            {
                _httpClient = httpClient;
            }

            if (cache == null)
            {
                _cache = new Cache();
            }
            else
            {
                _cache = cache;
            }
        }

        public void ChangeJsEngine(IJavaScriptEngine javaScriptEngine)
        {
            _javaScriptEngine = javaScriptEngine;
        }

        public Dictionary<string, string> ParseQuery(string query)
        {
            var parsed = HttpUtility.ParseQueryString(query);
            return parsed.AllKeys.ToDictionary(key => key, key => parsed[key]);
        }

        public string GetPlayerUrl(string webpage)
        {
            var regex = new Regex("\"(?:PLAYER_JS_URL|jsUrl)\"\\s*:\\s*\"([^\"]+)\"");
            var match = regex.Match(webpage);
            var playerUrl = match.Groups.Values.LastOrDefault()?.Value ?? "";
            if (playerUrl.StartsWith("//"))
            {
                return $"https:{playerUrl}";
            }
            else if (playerUrl.StartsWith("/"))
            {
                return $"https://www.youtube.com{playerUrl}";
            }
            return playerUrl;
        }

        public async Task<string> GetPlayerCode(string playerUrl)
        {
            var code = await _httpClient.GetStringAsync(playerUrl);
            return code;
        }

        public string GetPlayerId(string playerUrl)
        {
            var patterns = new[]
            {
                "/s/player/(?<id>[a-zA-Z0-9_-]{8,})/player",
                "/(?<id>[a-zA-Z0-9_-]{8,})/player(?:_ias\\.vflset(?:/[a-zA-Z]{2,3}_[a-zA-Z]{2,3})?|-plasma-ias-(?:phone|tablet)-[a-z]{2}_[A-Z]{2}\\.vflset)/base\\.js$",
                "\b(?<id>vfl[a-zA-Z0-9_-]+)\b.*?\\.js$"
            };
            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern);
                var match = regex.Match(playerUrl);
                if (match.Groups.TryGetValue("id", out var id))
                {
                    return id.Value;
                }
            }
            return "";
        }

        public async Task<string> GetStreamUrl(string signatureCipher, string playerUrl)
        {
            var playerId = GetPlayerId(playerUrl);
            string playerCode;
            if (!_cache.TryGetValue(playerId, out playerCode))
            {
                playerCode = await GetPlayerCode(playerUrl);
                _cache.Add(playerId, playerCode);
            }
            return await GetStreamUrl(signatureCipher, playerUrl, playerCode);
        }

        public async Task<string> GetStreamUrl(string signatureCipher, string playerUrl, string playerCode)
        {
            var playerId = GetPlayerId(playerUrl);

            var parsed = ParseQuery(signatureCipher);
            var url = parsed["url"];
            var sp = parsed["sp"];
            var encryptedSig = parsed["s"];

            var sigFuncName = _cache.GetOrAdd($"{playerId}-{nameof(ExtractSigFunctionName)}",
                () => ExtractSigFunctionName(playerCode));
            var sigFuncCode = _cache.GetOrAdd($"{playerId}-{nameof(ExtractSigFunctionCode)}",
                () => ExtractSigFunctionCode(sigFuncName, playerCode));

            var decryptedSig = await _javaScriptEngine.ExecuteJSCodeAsync(sigFuncCode, sigFuncName, encryptedSig);

            url = await ReplaceNWithDecrypted(url, playerId, playerCode);

            url = url + "&" + sp + "=" + decryptedSig;
            return url;
        }

        public async Task<string> ReplaceNWithDecrypted(string streamUrl, string playerUrl)
        {
            var playerId = GetPlayerId(playerUrl);
            string playerCode;
            if (!_cache.TryGetValue(playerId, out playerCode))
            {
                playerCode = await GetPlayerCode(playerUrl);
                _cache.Add(playerId, playerCode);
            }

            streamUrl = await ReplaceNWithDecrypted(streamUrl, playerId, playerCode);

            return streamUrl;
        }

        public async Task<string> ReplaceNWithDecrypted(string streamUrl, string playerId, string playerCode)
        {
            var encryptedN = HttpUtility.ParseQueryString(streamUrl)["n"];
            System.Diagnostics.Debug.WriteLine($"encryptedN {encryptedN}");
            if (encryptedN != null)
            {
                var decryptedN = await _cache.GetOrAddAsync($"{playerId}-{nameof(DecryptN)}-{encryptedN}",
                    () => DecryptN(encryptedN, playerId, playerCode));
                if (!string.IsNullOrEmpty(decryptedN))
                {
                    System.Diagnostics.Debug.WriteLine($"decryptedN {decryptedN}");
                    streamUrl = streamUrl.Replace($"n={encryptedN}", $"n={decryptedN}");
                }
            }

            return streamUrl;
        }

        public async Task<string> DecryptN(string encryptedN, string playerId, string playerCode)
        {
            var nFunctionName = _cache.GetOrAdd($"{playerId}-{nameof(ExtractNFunctionName)}",
                    () => ExtractNFunctionName(playerCode));
            var nFunctionCode = _cache.GetOrAdd($"{playerId}-{nameof(ExtractNFunctionCode)}",
                () => ExtractNFunctionCode(nFunctionName, playerCode));

            var decryptedN = await _javaScriptEngine.ExecuteJSCodeAsync(nFunctionCode, nFunctionName, encryptedN);
            return decryptedN;
        }

        public string ExtractSigFunctionName(string playerCode)
        {
            string result;

            var pattern1 = @"encodeURIComponent";
            var patterns1 = new[]
            {
                @"\b[cs]\s*&&\s*[adf]\.set\([^,]+\s*,\s*encodeURIComponent\s*\(\s*(?<sig>[a-zA-Z0-9$]+)\(",
                @"\b[a-zA-Z0-9]+\s*&&\s*[a-zA-Z0-9]+\.set\([^,]+\s*,\s*encodeURIComponent\s*\(\s*(?<sig>[a-zA-Z0-9$]+)\("
            };
            result = TwoPassMatch(pattern1, patterns1, playerCode);
            if (!string.IsNullOrEmpty(result)) return result;

            var pattern2 = @"\(decodeURIComponent\(h\.s\)\)";
            var patterns2 = new[]
            {
                @"\bm=(?<sig>[a-zA-Z0-9$]{2,})\(decodeURIComponent\(h\.s\)\)",
                @"\bc&&\(c=(?<sig>[a-zA-Z0-9$]{2,})\(decodeURIComponent\(c\)\)",
            };
            result = TwoPassMatch(pattern2, patterns2, playerCode);
            if (!string.IsNullOrEmpty(result)) return result;

            var pattern3 = @"=\s*function\(\s*a\s*\)\s*{\s*a\s*=\s*a\.split\(\s*""""\s*\)";
            var patterns3 = new[]
            {
                @"(?:\b|[^a-zA-Z0-9$])(?<sig>[a-zA-Z0-9$]{2,})\s*=\s*function\(\s*a\s*\)\s*{\s*a\s*=\s*a\.split\(\s*""""\s*\);[a-zA-Z0-9$]{2}\.[a-zA-Z0-9$]{2}\(a,\d+\)",
                @"(?:\b|[^a-zA-Z0-9$])(?<sig>[a-zA-Z0-9$]{2,})\s*=\s*function\(\s*a\s*\)\s*{\s*a\s*=\s*a\.split\(\s*""""\s*\)",
                @"(?<sig>[a-zA-Z0-9$]+)\s*=\s*function\(\s*a\s*\)\s*{\s*a\s*=\s*a\.split\(\s*""""\s*\)"
            };
            return TwoPassMatch(pattern3, patterns3, playerCode);
        }

        private string TwoPassMatch(string mainPattern, string[] patterns, string playerCode)
        {
            var regex = new Regex(mainPattern);
            var match = regex.Match(playerCode);

            while (match.Success)
            {
                foreach (var pattern in patterns)
                {
                    var sigRegex = new Regex(pattern);
                    var sigMatch = sigRegex.Match(playerCode, match.Index - 20, 100);
                    if (sigMatch.Groups.TryGetValue("sig", out Group group))
                    {
                        return group.Value;
                    }
                }

                match = match.NextMatch();
            }
            return "";
        }

        public string ExtractSigFunctionCode(string funcName, string playerCode)
        {
            var sigCode = "";
            var funcCode = ExtractFunctionCode(funcName, playerCode);
            var regexArg = new Regex(@"function\((?<arg>[\w]+)\)");
            var matchArg = regexArg.Match(funcCode);
            if (matchArg.Groups.TryGetValue("arg", out var group))
            {
                var argName = group.Value;
                var regexName = new Regex(@$"(?<name>[\w]+)[\.\w]+\({Regex.Escape(argName)},");
                var matchName = regexName.Match(funcCode);
                if (matchName.Groups.TryGetValue("name", out var groupName))
                {
                    var funcCode2 = ExtractVariableDefinition(groupName.Value, playerCode);
                    sigCode = $"{funcCode};{funcCode2}";
                }
            }
            return sigCode;
        }

        public string ExtractNFunctionName(string playerCode)
        {
            var regex = new Regex("\\.get\\(\"n\"\\)\\)&&\\(b=(?<nfunc>[a-zA-Z0-9$]+)(?:\\[(?<idx>\\d+)\\])?\\([a-zA-Z0-9]\\)");
            var match = regex.Match(playerCode);
            if (match.Groups.TryGetValue("nfunc", out var nFuncGroup))
            {
                if (match.Groups.TryGetValue("idx", out var idx) && idx.Value == "0")
                {
                    var regex2 = new Regex($"var {Regex.Escape(nFuncGroup.Value)}\\s*=\\s*(\\[.+?\\])\\s*[,;]");
                    var match2 = regex2.Match(playerCode);
                    var name = match2.Groups.Values.LastOrDefault()?.Value ?? "";
                    return name.Replace("[", "").Replace("]", "");
                }
                return nFuncGroup.Value;
            }

            string pattern = @"
                (?x)
                    (?:
                        \.get\(""n""\)\)&&\(b=|
                        (?:
                            b=String\.fromCharCode\(110\)|
                            (?<str_idx>[a-zA-Z0-9_$.]+)&&\(b=""nn""\[\+(\k<str_idx>)\]
                        )
                        (?:
                            ,[a-zA-Z0-9_$]+\(a\))?,c=a\.
                            (?:
                                get\(b\)|
                                [a-zA-Z0-9_$]+\[b\]\|\|null
                            )\)&&\(c=|
                        \b(?<var>[a-zA-Z0-9_$]+)=
                    )(?<nfunc>[a-zA-Z0-9_$]+)(?:\[(?<idx>\d+)\])?\([a-zA-Z]\)
                    (?(var),[a-zA-Z0-9_$]+\.set\(""n""\,(\k<var>)\),(\k<nfunc>)\.length)";
            regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);
            match = regex.Match(playerCode);
            if (match.Groups.TryGetValue("nfunc", out nFuncGroup))
            {
                if (match.Groups.TryGetValue("idx", out var idx) && idx.Value == "0")
                {
                    var regex2 = new Regex($"var {Regex.Escape(nFuncGroup.Value)}\\s*=\\s*(\\[.+?\\])\\s*[,;]");
                    var match2 = regex2.Match(playerCode);
                    var name = match2.Groups.Values.LastOrDefault()?.Value ?? "";
                    return name.Replace("[", "").Replace("]", "");
                }
                return nFuncGroup.Value;
            }

            return "";
        }

        public string ExtractFunctionCode(string functionName, string code)
        {
            var pattern = @$"{Regex.Escape(functionName)}\s*=\s*function\(\w+\)";
            var functionCode = GetCode(pattern, code);
            return functionCode;
        }

        public string ExtractNFunctionCode(string functionName, string code)
        {
            var pattern = @$"{Regex.Escape(functionName)}\s*=\s*function\(\w+\)";
            var regex = new Regex(pattern);
            var match = regex.Match(code);
            var functionCode = "";
            if (match.Success)
            {
                var lastIndex = code.IndexOf("enhanced_except", match.Index);
                lastIndex = code.IndexOf("return", lastIndex + "enhanced_except".Length);
                lastIndex = code.IndexOf("}", lastIndex, lastIndex + "return".Length);
                functionCode = code.Substring(match.Index, lastIndex - match.Index + 1);
            }

            return functionCode;
        }

        public string ExtractVariableDefinition(string varName, string code)
        {
            var pattern = @$"var\s*{Regex.Escape(varName)}\s*=";
            var varDef = GetCode(pattern, code);
            return varDef;
        }

        private string GetCode(string regexPattern, string code)
        {
            var regex = new Regex(regexPattern);
            var match = regex.Match(code);
            if (match.Success)
            {
                var startIndex = match.Index;
                int count = 0;
                var lastIndex = code.Length;
                bool ignore = false;
                for (int i = startIndex + match.Value.Length; i < code.Length; i++)
                {
                    if (code[i] == '"' && code[i - 1] != '\\')
                    {
                        ignore = !ignore;
                    }
                    if (code[i] == '{' && !ignore)
                    {
                        count++;
                    }
                    else if (code[i] == '}' && !ignore)
                    {
                        count--;
                    }
                    if (count == 0)
                    {
                        lastIndex = i;
                        break;
                    }
                }
                var functionCode = code.Substring(startIndex, lastIndex - startIndex + 1);
                return functionCode;
            }
            return "";
        }
    }
}