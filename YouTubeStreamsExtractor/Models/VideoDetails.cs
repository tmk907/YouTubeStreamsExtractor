using System.Text.Json.Serialization;

namespace YouTubeStreamsExtractor.Models
{
    public class VideoDetails
    {
        [JsonPropertyName("videoId")]
        public string VideoId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("lengthSeconds")]
        public string LengthSeconds { get; set; }

        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; }

        [JsonPropertyName("channelId")]
        public string ChannelId { get; set; }

        [JsonPropertyName("isOwnerViewing")]
        public bool IsOwnerViewing { get; set; }

        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonPropertyName("isCrawlable")]
        public bool IsCrawlable { get; set; }

        [JsonPropertyName("thumbnail")]
        public VideoThumbnails Thumbnail { get; set; }

        [JsonPropertyName("allowRatings")]
        public bool AllowRatings { get; set; }

        [JsonPropertyName("viewCount")]
        public string ViewCount { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonPropertyName("isUnpluggedCorpus")]
        public bool IsUnpluggedCorpus { get; set; }

        [JsonPropertyName("isLiveContent")]
        public bool IsLiveContent { get; set; }
    }

    public class VideoThumbnails
    {
        [JsonPropertyName("thumbnails")]
        public List<Thumbnail> Thumbnails { get; set; }
    }

    public class Thumbnail
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }


}
