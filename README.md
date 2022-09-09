# YouTubeStreamsExtractor

YouTubeStreamsExtractor is a library for extracting url for downloading audio and video streams from YouTube videos. It's based on [yt-dlp](https://github.com/yt-dlp/yt-dlp)

## Install

Install YouTubeStreamsExtractor and one of the javascript engines or implement `IJavaScriptEngine` interface.

| Version | Package | Description |
| ------- | ------- | ----------- |
| [![Nuget](https://img.shields.io/nuget/v/YouTubeStreamsExtractor)](https://www.nuget.org/packages/YouTubeStreamsExtractor) | [YouTubeStreamsExtractor](https://www.nuget.org/packages/YouTubeStreamsExtractor) | .NET 6 Library |
| [![Nuget](https://img.shields.io/nuget/v/YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher)](https://www.nuget.org/packages/YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher) | [YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher](https://www.nuget.org/packages/YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher) | javascript engine, .NET 6 Library |
| [![Nuget](https://img.shields.io/nuget/v/YouTubeStreamsExtractor.JsEngine.AndroidWebView)](https://www.nuget.org/packages/YouTubeStreamsExtractor.JsEngine.AndroidWebView) | [YouTubeStreamsExtractor.JsEngine.AndroidWebView](https://www.nuget.org/packages/YouTubeStreamsExtractor.JsEngine.AndroidWebView) | javascript engine, .NET 6 Android Library |

## Usage

Get streams with playable urls
```c#
using YouTubeStreamsExtractor;
using YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher;

IJavaScriptEngine jsEngine = new JavaScriptJurassicEngine();
var youTubeStreams = new YouTubeStreams(jsEngine);
```

Find audio with highest bitrate
```c#
var streams = await youTubeStreams.GetAllStreamsAsync(videoUrl, true);
var streamSelector = new StreamSelector();
var audio = streamSelector.SelectBestAudio(streams);

var downloadUrl = audio.PlayableUrl.Url;
```

If you only need a URL for one stream, use this method. 
It doesn't need to prepare the url for each stream, which makes it faster.
```c#
var streams = await youTubeStreams.GetAllStreamsAsync(url, false);
var audio = streamSelector.SelectBestAudio(streams2);
await audio.PlayableUrl.PrepareAsync(audio.RawUrl, youTubeStreams.Decryptor);

var downloadUrl = audio.PlayableUrl.Url;
```

## YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher
Uses [JavaScript Engine Switcher](https://github.com/Taritsyn/JavaScriptEngineSwitcher) with `Jurassic` and `NiL.JS` engines to execute js code.  
This is the simplest implementation but sometimes it might not be able to execute js code to descramble 'n' parameter, this will result in throttled download speed.

## YouTubeStreamsExtractor.JsEngine.AndroidWebView
Uses WebView to execute js code.