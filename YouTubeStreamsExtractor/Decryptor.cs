using System.Text.RegularExpressions;
using System.Web;

namespace YouTubeStreamsExtractor
{
    public class Decryptor
    {
        private readonly HttpClient _httpClient;
        private readonly ICache _cache;

        public Decryptor()
        {
            _httpClient = new HttpClient();
            _cache = new Cache();
        }

        public Decryptor(HttpClient httpClient) 
        {
            _httpClient = httpClient;
            _cache = new Cache();
        }        

        public Decryptor(HttpClient httpClient, ICache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public Dictionary<string,string> ParseQuery(string query)
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
            foreach(var pattern in patterns)
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
            return GetStreamUrl(signatureCipher, playerUrl, playerCode);
        }

        public string GetStreamUrl(string signatureCipher, string playerUrl, string playerCode)
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

            var decryptedSig = ExecuteJSCode(sigFuncCode, sigFuncName, encryptedSig);

            url = ReplaceNWithDecrypted(url, playerId, playerCode);

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

            streamUrl = ReplaceNWithDecrypted(streamUrl, playerId, playerCode);

            return streamUrl;
        }

        public string ReplaceNWithDecrypted(string streamUrl, string playerId, string playerCode)
        {
            var encryptedN = HttpUtility.ParseQueryString(streamUrl)["n"];
            if (encryptedN != null)
            {
                var decryptedN = _cache.GetOrAdd($"{playerId}-{nameof(DecryptN)}-{encryptedN}",
                    () => DecryptN(encryptedN, playerId, playerCode));
                if (!string.IsNullOrEmpty(decryptedN))
                {
                    streamUrl = streamUrl.Replace($"n={encryptedN}", $"n={decryptedN}");
                }
            }

            return streamUrl;
        }

        public string DecryptN(string encryptedN, string playerId, string playerCode)
        {
            var nFunctionName = _cache.GetOrAdd($"{playerId}-{nameof(ExtractNFunctionName)}",
                    () => ExtractNFunctionName(playerCode));
            var nFunctionCode = _cache.GetOrAdd($"{playerId}-{nameof(ExtractFunctionCode)}",
                () => ExtractFunctionCode(nFunctionName, playerCode));

            var decryptedN = ExecuteJSCode(nFunctionCode, nFunctionName, encryptedN);
            return decryptedN;
        }

        public string ExtractSigFunctionName(string playerCode)
        {
            var patterns = new[]
            {
                @"\b[cs]\s*&&\s*[adf]\.set\([^,]+\s*,\s*encodeURIComponent\s*\(\s*(?<sig>[a-zA-Z0-9$]+)\(",
                @"\b[a-zA-Z0-9]+\s*&&\s*[a-zA-Z0-9]+\.set\([^,]+\s*,\s*encodeURIComponent\s*\(\s*(?<sig>[a-zA-Z0-9$]+)\(",
                @"\bm=(?<sig>[a-zA-Z0-9$]{2,})\(decodeURIComponent\(h\.s\)\)",
                @"\bc&&\(c=(?<sig>[a-zA-Z0-9$]{2,})\(decodeURIComponent\(c\)\)",
                @"(?:\b|[^a-zA-Z0-9$])(?<sig>[a-zA-Z0-9$]{2,})\s*=\s*function\(\s*a\s*\)\s*{\s*a\s*=\s*a\.split\(\s*""""\s*\);[a-zA-Z0-9$]{2}\.[a-zA-Z0-9$]{2}\(a,\d+\)",
                @"(?:\b|[^a-zA-Z0-9$])(?<sig>[a-zA-Z0-9$]{2,})\s*=\s*function\(\s*a\s*\)\s*{\s*a\s*=\s*a\.split\(\s*""""\s*\)",
                @"(?<sig>[a-zA-Z0-9$]+)\s*=\s*function\(\s*a\s*\)\s*{\s*a\s*=\s*a\.split\(\s*""""\s*\)"
            };
            foreach(var pattern in patterns)
            {
                var regex = new Regex(pattern);
                var match = regex.Match(playerCode);
                if (match.Groups.TryGetValue("sig", out Group group))
                {
                    return group.Value;
                }
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
                    var regex2 = new Regex($"var {Regex.Escape(nFuncGroup.Value)}\\s*=\\s*(\\[.+?\\]);");
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
            var pattern = @$"{functionName}\s*=\s*function\(\w+\)";
            var functionCode = GetCode(pattern, code);
            return functionCode;
        }

        public string ExtractVariableDefinition(string varName, string code)
        {
            var pattern = @$"var\s*{varName}\s*=";
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

        public string ExecuteJSCode(string code, string functionName, string argument)
        {
            try
            {
                var jengine = new Jurassic.ScriptEngine();
                jengine.Evaluate(code);
                string result = jengine.CallGlobalFunction(functionName, argument) as string;
                return result ?? "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}