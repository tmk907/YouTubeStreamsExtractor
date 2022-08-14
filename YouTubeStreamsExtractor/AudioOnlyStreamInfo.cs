namespace YouTubeStreamsExtractor
{
    public class AudioOnlyStreamInfo : IAudioOnlyStreamInfo
    {
        public string AudioQuality { get; init; }

        public int AudioSampleRate { get; init; }

        public int AudioChannels { get; init; }

        public int ITag { get; init; }

        public string Url { get; set; }

        public string MimeType { get; init; }

        public string Codecs { get; init; }

        public long Bitrate { get; init; }

        public TimeSpan Duration { get; init; }

        public long ContentLength { get; init; }
    }
}
