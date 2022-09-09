using Android.OS;

namespace Xamarin.Essentials
{
    public static partial class MainThread
    {
        static volatile Handler handler;

        static bool PlatformIsMainThread
        {
            get
            {
                if (Platform.HasApiLevel(BuildVersionCodes.M))
                    return Looper.MainLooper.IsCurrentThread;

                return Looper.MyLooper() == Looper.MainLooper;
            }
        }

        static void PlatformBeginInvokeOnMainThread(Action action)
        {
            if (handler?.Looper != Looper.MainLooper)
                handler = new Handler(Looper.MainLooper);

            handler.Post(action);
        }
    }

    public static partial class Platform
    {
        static int? sdkInt;

        internal static int SdkInt
            => sdkInt ??= (int)Build.VERSION.SdkInt;

        internal static bool HasApiLevel(BuildVersionCodes versionCode) =>
            SdkInt >= (int)versionCode;
    }
}