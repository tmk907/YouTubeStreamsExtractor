using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jurassic;

namespace YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher
{
    public class JavaScriptJurassicEngine : IJavaScriptEngine
    {
        public JavaScriptJurassicEngine()
        {
            IJsEngineSwitcher engineSwitcher = JsEngineSwitcher.Current;
            engineSwitcher.EngineFactories.AddJurassic();
            engineSwitcher.DefaultEngineName = JurassicJsEngine.EngineName;
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
