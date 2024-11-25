﻿using YouTubeStreamsExtractor;
using YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher;

IJavaScriptEngine jsEngine = new JavaScriptJurassicEngine();
var youTubeStreams = new YouTubeStreams(jsEngine);
var streamSelector = new StreamSelector();

var url = "https://www.youtube.com/watch?v=gNn9NxZH2Vo";

Console.WriteLine("Get streams with playable urls");
try
{
	// get all streams with 
	var streams = await youTubeStreams.GetAllStreamsAsync(url, true);
	Console.WriteLine($"Found {streams.Count()} streams");

	var audio = streamSelector.SelectBestAudio(streams);
	Console.WriteLine($"Found audio with best quality {audio.ITag} {audio.Container} {audio.Bitrate}");
	Console.WriteLine($"{audio.PlayableUrl.Url}");


	Console.WriteLine("Get streams wihout playable urls");
	var streams2 = await youTubeStreams.GetAllStreamsAsync(url, false);
	Console.WriteLine($"Found {streams2.Count()} streams");

	var audio2 = streamSelector.SelectBestAudio(streams2);
	Console.WriteLine($"Audio with best quality {audio2.ITag} {audio2.Container} {audio2.Bitrate}");
	Console.WriteLine($"{audio2.PlayableUrl.Url}");
	await audio2.PlayableUrl.PrepareAsync(audio2.RawUrl, youTubeStreams.Decryptor);
	Console.WriteLine($"Audio with best quality {audio2.ITag} {audio2.Container} {audio2.Bitrate}");
	Console.WriteLine($"{audio2.PlayableUrl.Url}");
}
catch (Exception ex)
{

}

Console.WriteLine("Done");
