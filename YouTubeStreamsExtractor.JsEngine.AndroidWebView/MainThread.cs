using Android.OS;

namespace YouTubeStreamsExtractor.AndroidWebViewJsEngine
{
    internal static class MainThread
    {
        static volatile Handler handler;

        public static void BeginInvokeOnMainThread(Action action)
        {
            if (Looper.MainLooper.IsCurrentThread)
            {
                action();
            }
            else
            {
                if (handler?.Looper != Looper.MainLooper)
                {
                    handler = new Handler(Looper.MainLooper);
                }

                handler.Post(action);
            }
        }
    }
}