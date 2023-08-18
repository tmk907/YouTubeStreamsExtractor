using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;

namespace YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher
{
    public class JavaScriptJintEngine : IJavaScriptEngine
    {
        public JavaScriptJintEngine()
        {
            IJsEngineSwitcher engineSwitcher = JsEngineSwitcher.Current;
            engineSwitcher.EngineFactories.AddJint();
            engineSwitcher.DefaultEngineName = JintJsEngine.EngineName;
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
