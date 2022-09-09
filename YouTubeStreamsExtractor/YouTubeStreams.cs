using YouTubeStreamsExtractor.Models;
using System.Text.Json;

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
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");
                _httpClient.DefaultRequestHeaders.Add("Cookie", "CONSENT=Yes+cb; YSC=NYW5raUzYP4; ");
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

            var token1 = "var ytInitialPlayerResponse = ";
            var indexOfVar = response.IndexOf(token1);
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
                if (count == 0)
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
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");
            httpClient.DefaultRequestHeaders.Remove("Cookie"); 
            httpClient.DefaultRequestHeaders.Add("Cookie", "CONSENT=Yes+cb; YSC=NYW5raUzYP4; ");
        }
    }
}
