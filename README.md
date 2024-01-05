# YouTubeStreamsExtractor

YouTubeStreamsExtractor is a library for extracting url for downloading audio and video streams from YouTube videos. It's inspired by [yt-dlp](https://github.com/yt-dlp/yt-dlp)

## Install

Install YouTubeStreamsExtractor and one of the javascript engines or implement `IJavaScriptEngine` interface.

| Version | Package | Description |
| ------- | ------- | ----------- |
| [![Nuget](https://img.shields.io/nuget/v/YouTubeStreamsExtractor)](https://www.nuget.org/packages/YouTubeStreamsExtractor) | [YouTubeStreamsExtractor](https://www.nuget.org/packages/YouTubeStreamsExtractor) | .NET 8 Library |
| [![Nuget](https://img.shields.io/nuget/v/YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher)](https://www.nuget.org/packages/YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher) | [YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher](https://www.nuget.org/packages/YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher) | javascript engine, .NET 8 Library |
| [![Nuget](https://img.shields.io/nuget/v/YouTubeStreamsExtractor.JsEngine.AndroidWebView)](https://www.nuget.org/packages/YouTubeStreamsExtractor.JsEngine.AndroidWebView) | [YouTubeStreamsExtractor.JsEngine.AndroidWebView](https://www.nuget.org/packages/YouTubeStreamsExtractor.JsEngine.AndroidWebView) | javascript engine, .NET 8 Android Library |

## Usage

### Get streams with playable urls
```c#
using YouTubeStreamsExtractor;
using YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher;

IJavaScriptEngine jsEngine = new JavaScriptJintEngine();
var youTubeStreams = new YouTubeStreams(jsEngine);
var streams = await youTubeStreams.GetAllStreamsAsync(videoUrl, prepareAllUrls: true);
```

If you only need a URL for one stream, set `prepareAllUrls` to `false` and use `PrepareAsync()` method. 
It doesn't need to prepare the url for each stream, which makes it faster.
```c#
var streams = await youTubeStreams.GetAllStreamsAsync(url, prepareAllUrls: false);
var audio = streamSelector.SelectBestAudio(streams);
await audio.PlayableUrl.PrepareAsync(audio.RawUrl, youTubeStreams.Decryptor);

var downloadUrl = audio.PlayableUrl.Url;
```

### Find audio with highest bitrate
```c#
var streamSelector = new StreamSelector();
var audio = streamSelector.SelectBestAudio(streams);

var downloadUrl = audio.PlayableUrl.Url;
```

### Find videos that match specific resolutions and/or codecs.

This will return stream with highest bitrate that has resolution 1080p and codec vp9 or null if that stream is not available.
```c#
var streamSelector = new StreamSelector(new[] { "1080p" }, new[] { "vp9" });
IVideoStreamInfo? videoStream = streamSelector.SelectPreferredVideo(streams);
```

This will return 1080p stream with highest bitrate. If it is not available, then 1080p60, then 720p and 720p60.
```c#
var streamSelector = new StreamSelector(new[] { "1080p", "1080p60", "720p", "720p60" });
IVideoStreamInfo? videoStream = streamSelector.SelectPreferredVideo(streams);
```

## YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher
Uses [JavaScript Engine Switcher](https://github.com/Taritsyn/JavaScriptEngineSwitcher) with `Jint`, `Jurassic` and `NiL.JS` engines to execute js code.  
This is the simplest implementation but sometimes it might not be able to execute js code to descramble 'n' parameter, which will result in throttled download speed.

## YouTubeStreamsExtractor.JsEngine.AndroidWebView
Uses WebView to execute js code.

## ExampleWinUI3App
WinUI3 example application uses WebView2 to execute javascript code.
