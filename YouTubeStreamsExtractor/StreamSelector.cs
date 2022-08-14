namespace YouTubeStreamsExtractor
{
    public class StreamSelector
    {
        public static List<string> DefaultVideoCodecPreferences =
            new List<string> { "av01", "vp9", "avc1" };
        public static List<string> DefaultResolutionPreferences =
            new List<string> { "1080p", "1080p60", "720p", "720p60", "480p", "360p", "240p" };


        private readonly List<string> _videoCodecPreferences = new List<string>();

        private readonly List<string> _resolutionPreferences = new List<string>();

        public StreamSelector()
        {
            _videoCodecPreferences.AddRange(DefaultVideoCodecPreferences);
            _resolutionPreferences.AddRange(DefaultResolutionPreferences);
        }

        public StreamSelector(IEnumerable<string> resPreferences, IEnumerable<string> codecPreferences)
        {
            _videoCodecPreferences.AddRange(codecPreferences);
            _resolutionPreferences.AddRange(resPreferences);
        }

        public IVideoStreamInfo? SelectPreferredVideo(IEnumerable<IStreamInfo> streams,
            string? container = null, bool allowAnyCodec = false)
        {
            if (allowAnyCodec || _videoCodecPreferences.Count == 0)
            {
                foreach (var res in _resolutionPreferences)
                {
                    var stream = streams.OfType<IVideoStreamInfo>()
                        .Where(x => IsMatch(x, res, container, null))
                        .OrderByDescending(x => x.Bitrate)
                        .FirstOrDefault();
                    if (stream != null) return stream;
                }
            }
            else
            {
                foreach (var res in _resolutionPreferences)
                {
                    foreach (var codec in _videoCodecPreferences)
                    {
                        var stream = streams.OfType<IVideoStreamInfo>()
                            .Where(x => IsMatch(x, res, container, codec))
                            .OrderByDescending(x => x.Bitrate)
                            .FirstOrDefault();
                        if (stream != null) return stream;
                    }
                }
            }
            return null;
        }

        public IAudioStreamInfo? SelectBestAudio(IEnumerable<IStreamInfo> streams, string? container = null)
        {
            return streams
                .OfType<IAudioOnlyStreamInfo>()
                .Where(x => string.IsNullOrEmpty(container) || x.Container == container)
                .OrderByDescending(x => x.Bitrate)
                .FirstOrDefault();
        }

        private bool IsMatch(IVideoStreamInfo stream, string? qualityLabel = null,
            string? container = null, string? codec = null)
        {
            return (stream.QualityLabel == qualityLabel || string.IsNullOrEmpty(qualityLabel))
                && (stream.Container == container || string.IsNullOrEmpty(container))
                && (stream.Codec == codec || string.IsNullOrEmpty(codec));
        }
    }
}
