using System.Text.Json.Serialization;

namespace YouTubeStreamsExtractor.Models
{
    public class AdaptiveFormat
    {
        [JsonPropertyName("itag")]
        public int Itag { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }

        [JsonPropertyName("bitrate")]
        public int Bitrate { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("initRange")]
        public InitRange InitRange { get; set; }

        [JsonPropertyName("indexRange")]
        public IndexRange IndexRange { get; set; }

        [JsonPropertyName("lastModified")]
        public string LastModified { get; set; }

        [JsonPropertyName("contentLength")]
        public long? ContentLength { get; set; }

        [JsonPropertyName("quality")]
        public string Quality { get; set; }

        [JsonPropertyName("fps")]
        public int Fps { get; set; }

        [JsonPropertyName("qualityLabel")]
        public string QualityLabel { get; set; }

        [JsonPropertyName("projectionType")]
        public string ProjectionType { get; set; }

        [JsonPropertyName("averageBitrate")]
        public int AverageBitrate { get; set; }

        [JsonPropertyName("approxDurationMs")]
        public long ApproxDurationMs { get; set; }

        [JsonPropertyName("colorInfo")]
        public ColorInfo ColorInfo { get; set; }

        [JsonPropertyName("highReplication")]
        public bool? HighReplication { get; set; }

        [JsonPropertyName("audioQuality")]
        public string AudioQuality { get; set; }

        [JsonPropertyName("audioSampleRate")]
        public int AudioSampleRate { get; set; }

        [JsonPropertyName("audioChannels")]
        public int AudioChannels { get; set; }

        [JsonPropertyName("loudnessDb")]
        public double? LoudnessDb { get; set; }

        [JsonPropertyName("signatureCipher")]
        public string? SignatureCipher { get; set; }
    }

    public class ColorInfo
    {
        [JsonPropertyName("primaries")]
        public string Primaries { get; set; }

        [JsonPropertyName("transferCharacteristics")]
        public string TransferCharacteristics { get; set; }

        [JsonPropertyName("matrixCoefficients")]
        public string MatrixCoefficients { get; set; }
    }

    public class Format
    {
        [JsonPropertyName("itag")]
        public int Itag { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }

        [JsonPropertyName("bitrate")]
        public int Bitrate { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("lastModified")]
        public string LastModified { get; set; }

        [JsonPropertyName("contentLength")]
        public long? ContentLength { get; set; }

        [JsonPropertyName("quality")]
        public string Quality { get; set; }

        [JsonPropertyName("fps")]
        public int Fps { get; set; }

        [JsonPropertyName("qualityLabel")]
        public string QualityLabel { get; set; }

        [JsonPropertyName("projectionType")]
        public string ProjectionType { get; set; }

        [JsonPropertyName("averageBitrate")]
        public int AverageBitrate { get; set; }

        [JsonPropertyName("audioQuality")]
        public string AudioQuality { get; set; }

        [JsonPropertyName("approxDurationMs")]
        public long ApproxDurationMs { get; set; }

        [JsonPropertyName("audioSampleRate")]
        public int AudioSampleRate { get; set; }

        [JsonPropertyName("audioChannels")]
        public int AudioChannels { get; set; }

        [JsonPropertyName("signatureCipher")]
        public string? SignatureCipher { get; set; }
    }

    public class IndexRange
    {
        [JsonPropertyName("start")]
        public string Start { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }
    }

    public class InitRange
    {
        [JsonPropertyName("start")]
        public string Start { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }
    }

    public class StreamingData
    {
        [JsonPropertyName("expiresInSeconds")]
        public string ExpiresInSeconds { get; set; }

        [JsonPropertyName("formats")]
        public List<Format> Formats { get; set; }

        [JsonPropertyName("adaptiveFormats")]
        public List<AdaptiveFormat> AdaptiveFormats { get; set; }
    }
}
