namespace YouTubeStreamsExtractor
{
    public interface IDecryptor
    {
        Task<string> GetPlayerCode(string playerUrl);
        string GetPlayerUrl(string webpage);
        Task<string> GetStreamUrl(string signatureCipher, string playerUrl);
        Task<string> GetStreamUrl(string signatureCipher, string playerUrl, string playerCode);
        Task<string> ReplaceNWithDecrypted(string streamUrl, string playerUrl);
        void ChangeJsEngine(IJavaScriptEngine javaScriptEngine);
    }
}