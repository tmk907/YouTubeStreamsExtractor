namespace YouTubeStreamsExtractor
{
    public interface IJavaScriptEngine
    {
        Task<string> ExecuteJSCodeAsync(string code, string functionName, string argument);
    }
}
