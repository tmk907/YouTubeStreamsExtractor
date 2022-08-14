using System.Text.Json.Serialization;

namespace YouTubeStreamsExtractor.Models
{
    public class YTInitialPlayerResponse
    {
        [JsonPropertyName("streamingData")]
        public StreamingData StreamingData { get; set; }

        [JsonPropertyName("videoDetails")]
        public VideoDetails VideoDetails { get; set; }

        [JsonPropertyName("captions")]
        public Captions Captions { get; set; }
    }
}
