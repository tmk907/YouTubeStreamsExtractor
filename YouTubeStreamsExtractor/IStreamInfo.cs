namespace YouTubeStreamsExtractor
{
    public interface IStreamInfo
    {
        public int ITag { get; }
        public string? Url { get; }
        public string MimeType { get; }
        public string Codecs { get; }
        public long Bitrate { get; }
        public TimeSpan Duration { get; }
        public long ContentLength { get; }

        public string StreamType => MimeType.Split('/')[0];
        public string Codec => Codecs.Split('.')[0].Replace("codecs", "").Replace("\"", "").Replace("=", "").Trim();
        public string Container => MimeType.Split('/')[1];
    }

    public interface IAudioStreamInfo : IStreamInfo
    {
        public string AudioQuality { get; }
        public int AudioSampleRate { get; }
        public int AudioChannels { get; }
    }

    public interface IVideoStreamInfo : IStreamInfo
    {
        public int Width { get; }
        public int Height { get; }
        public int Fps { get; }
        public string QualityLabel { get; }
    }
    
    public interface IAudioOnlyStreamInfo : IAudioStreamInfo { }
    
    public interface IVideoOnlyStreamInfo : IVideoStreamInfo { }

    public interface IMuxedStreamInfo : IAudioStreamInfo, IVideoStreamInfo { }
}
