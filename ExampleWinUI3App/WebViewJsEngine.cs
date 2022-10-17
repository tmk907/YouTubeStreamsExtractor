using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;
using YouTubeStreamsExtractor;

namespace ExampleWinUI3App
{
    public class WebViewJsEngine : IJavaScriptEngine
    {
        private readonly CoreWebView2 _webView;

        public WebViewJsEngine(CoreWebView2 webView)
        {
            _webView = webView;
        }

        public async Task<string> ExecuteJSCodeAsync(string code, string functionName, string argument)
        {
            var script = $"{code}; {functionName}('{argument}')";

            var result = await _webView.ExecuteScriptAsync(script);

            return result.Replace("\"", "");
        }
    }
}
