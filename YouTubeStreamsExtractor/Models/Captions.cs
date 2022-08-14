using System.Text.Json.Serialization;

namespace YouTubeStreamsExtractor.Models
{
    public class AudioTrack
    {
        [JsonPropertyName("captionTrackIndices")]
        public List<int> CaptionTrackIndices { get; set; }
    }

    public class Captions
    {
        [JsonPropertyName("playerCaptionsTracklistRenderer")]
        public PlayerCaptionsTracklistRenderer PlayerCaptionsTracklistRenderer { get; set; }
    }

    public class CaptionTrack
    {
        [JsonPropertyName("baseUrl")]
        public string BaseUrl { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("vssId")]
        public string VssId { get; set; }

        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("isTranslatable")]
        public bool IsTranslatable { get; set; }
    }

    public class LanguageName
    {
        [JsonPropertyName("simpleText")]
        public string SimpleText { get; set; }
    }

    public class Name
    {
        [JsonPropertyName("simpleText")]
        public string SimpleText { get; set; }
    }

    public class PlayerCaptionsTracklistRenderer
    {
        [JsonPropertyName("captionTracks")]
        public List<CaptionTrack> CaptionTracks { get; set; }

        [JsonPropertyName("audioTracks")]
        public List<AudioTrack> AudioTracks { get; set; }

        [JsonPropertyName("translationLanguages")]
        public List<TranslationLanguage> TranslationLanguages { get; set; }

        [JsonPropertyName("defaultAudioTrackIndex")]
        public int DefaultAudioTrackIndex { get; set; }
    }

    public class TranslationLanguage
    {
        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; }

        [JsonPropertyName("languageName")]
        public LanguageName LanguageName { get; set; }
    }
}
