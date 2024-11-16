using YouTubeStreamsExtractor.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace YouTubeStreamsExtractor
{
    public class YouTubeStreams
    {
        private readonly HttpClient _httpClient;
        private readonly Decryptor _decryptor;
        private readonly IJavaScriptEngine _javaScriptEngine;

        public IDecryptor Decryptor => _decryptor;

        public YouTubeStreams(IJavaScriptEngine javaScriptEngine, HttpClient? httpClient = null, ICache? cache = null)
        {
            _javaScriptEngine = javaScriptEngine;

            if (httpClient == null)
            {
                var msgHandler = new HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.All
                };
                _httpClient = new HttpClient(msgHandler);
                _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "com.google.ios.youtube/19.29.1 (iPhone16,2; U; CPU iOS 17_5_1 like Mac OS X;)");
                _httpClient.DefaultRequestHeaders.Add("X-YouTube-Client-Name", "IOS");
                _httpClient.DefaultRequestHeaders.Add("X-YouTube-Client-Version", "19.29.1");
            }
            else
            {
                _httpClient = httpClient;
            }

            _decryptor = new Decryptor(_javaScriptEngine, _httpClient, cache);
        }

        public async Task<IEnumerable<IStreamInfo>> GetAllStreamsAsync(string url, bool prepareAllUrls = false)
        {
            var youTubeData = await GetPlayerResponseAsync(url);
            var streams = await GetAllStreamsAsync(youTubeData, prepareAllUrls);
            return streams;
        }

        public async Task<YouTubeData> GetPlayerResponseAsync(string url)
        {
            var response = await _httpClient.GetStringAsync(url);

            var regex = new Regex("var ytInitialPlayerResponse\\s*=\\s*{", RegexOptions.Compiled);
            var match = regex.Match(response);
            var token1 = "var ytInitialPlayerResponse =";
            var indexOfVar = match.Index;
            var startIndex = indexOfVar + token1.Length;
            var lastIndex = response.Length;
            var count = 0;
            for (int i = startIndex; i < response.Length; i++)
            {
                if (response[i] == '{')
                {
                    count++;
                }
                else if (response[i] == '}')
                {
                    count--;
                }
                if (count == 0 && i > startIndex + 10)
                {
                    lastIndex = i;
                    break;
                }
            }
            var youtubeData = new YouTubeData();
            if (lastIndex != response.Length)
            {
                var json = response.Substring(startIndex, lastIndex - startIndex + 1);
                var serializerOptions = new JsonSerializerOptions
                {
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
                };
                youtubeData.YTInitialPlayerResponse = JsonSerializer.Deserialize<YTInitialPlayerResponse>(json, serializerOptions);
            }

            youtubeData.PlayerUrl = _decryptor.GetPlayerUrl(response);

            return youtubeData;
        }

        public async Task<IEnumerable<IStreamInfo>> GetAllStreamsAsync(YouTubeData youTubeData, bool prepareAllUrls = false)
        {
            var streams = new List<IStreamInfo>();

            var adaptiveFormats = youTubeData?.YTInitialPlayerResponse?.StreamingData?.AdaptiveFormats;
            if (adaptiveFormats != null)
            {
                foreach(var x in adaptiveFormats.Where(x => x.MimeType.Contains("audio")))
                {
                    var streamInfo = new AudioOnlyStreamInfo
                    {
                        AudioChannels = x.AudioChannels,
                        AudioQuality = x.AudioQuality,
                        AudioSampleRate = x.AudioSampleRate,

                        Bitrate = x.Bitrate,
                        Codecs = x.MimeType.Split(';')[1],
                        ContentLength = x.ContentLength ?? 0,
                        Duration = TimeSpan.FromMilliseconds(x.ApproxDurationMs),
                        ITag = x.Itag,
                        MimeType = x.MimeType.Split(';')[0],
                        RawUrl = x.Url,

                        PlayableUrl = new PlayableUrl(x.SignatureCipher, youTubeData.PlayerUrl)
                    };
                    streams.Add(streamInfo);
                }

                foreach (var x in adaptiveFormats.Where(x => x.MimeType.Contains("video")))
                {
                    var streamInfo = new VideoOnlyStreamInfo
                    {
                        Bitrate = x.Bitrate,
                        Codecs = x.MimeType.Split(';')[1],
                        ContentLength = x.ContentLength ?? 0,
                        Duration = TimeSpan.FromMilliseconds(x.ApproxDurationMs),
                        ITag = x.Itag,
                        MimeType = x.MimeType.Split(';')[0],
                        RawUrl = x.Url,

                        Fps = x.Fps,
                        Height = x.Height,
                        QualityLabel = x.QualityLabel,
                        Width = x.Width,

                        PlayableUrl = new PlayableUrl(x.SignatureCipher, youTubeData.PlayerUrl)
                    };
                    streams.Add(streamInfo);
                }
            }

            var formats = youTubeData?.YTInitialPlayerResponse?.StreamingData?.Formats;
            if (formats != null)
            {
                foreach (var x in formats)
                {
                    var streamInfo = new MuxedStreamInfo
                    {
                        AudioChannels = x.AudioChannels,
                        AudioQuality = x.AudioQuality,
                        AudioSampleRate = x.AudioSampleRate,

                        Bitrate = x.Bitrate,
                        Codecs = x.MimeType.Split(';')[1],
                        ContentLength = x.ContentLength ?? 0,
                        Duration = TimeSpan.FromMilliseconds(x.ApproxDurationMs),
                        ITag = x.Itag,
                        MimeType = x.MimeType.Split(';')[0],
                        RawUrl = x.Url,

                        Fps = x.Fps,
                        Height = x.Height,
                        QualityLabel = x.QualityLabel,
                        Width = x.Width,

                        PlayableUrl = new PlayableUrl(x.SignatureCipher, youTubeData.PlayerUrl)
                    };
                    streams.Add(streamInfo);
                }
            }

            if (prepareAllUrls)
            {
                foreach (var stream in streams)
                {
                    await stream.PlayableUrl.PrepareAsync(stream.RawUrl, _decryptor);
                }
            }

            return streams;
        }

        public static void ReplaceReqiredHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Remove("Accept");
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Remove("User-Agent");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "com.google.ios.youtube/19.29.1 (iPhone16,2; U; CPU iOS 17_5_1 like Mac OS X;)");
            httpClient.DefaultRequestHeaders.Remove("Cookie"); 
            httpClient.DefaultRequestHeaders.Add("X-YouTube-Client-Name", "IOS");
            httpClient.DefaultRequestHeaders.Add("X-YouTube-Client-Version", "19.29.1");
        }
    }
}
