namespace YouTubeStreamsExtractor
{
    public class VideoOnlyStreamInfo : IVideoOnlyStreamInfo
    {
        public int Width { get; init; }

        public int Height { get; init; }

        public int Fps { get; init; }

        public string QualityLabel { get; init; }

        public int ITag { get; init; }

        public string RawUrl { get; init; }

        public string MimeType { get; init; }

        public string Codecs { get; init; }

        public long Bitrate { get; init; }

        public TimeSpan Duration { get; init; }

        public long ContentLength { get; init; }

        public PlayableUrl PlayableUrl { get; init; }
    }
}
