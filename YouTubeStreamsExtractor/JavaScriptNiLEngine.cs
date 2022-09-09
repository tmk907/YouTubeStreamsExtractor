using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.NiL;

namespace YouTubeStreamsExtractor
{
    public class JavaScriptNiLEngine : IJavaScriptEngine
    {
        public JavaScriptNiLEngine()
        {
            IJsEngineSwitcher engineSwitcher = JsEngineSwitcher.Current;
            engineSwitcher.EngineFactories.AddNiL();
            engineSwitcher.DefaultEngineName = NiLJsEngine.EngineName;
        }

        public Task<string> ExecuteJSCodeAsync(string code, string functionName, string argument)
        {
            try
            {
                IJsEngine engine = JsEngineSwitcher.Current.CreateDefaultEngine();
                engine.Execute(code);
                string result = engine.CallFunction(functionName, argument) as string;
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                return Task.FromResult("");
            }
        }
    }
}
