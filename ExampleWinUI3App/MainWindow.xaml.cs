using Microsoft.UI.Xaml;
using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using YouTubeStreamsExtractor;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ExampleWinUI3App
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private IJavaScriptEngine jsEngine;
        private YouTubeStreams _youTubeStreams;

        public MainWindow()
        {
            this.InitializeComponent();
            webview.Loaded += Webview_Loaded;
        }

        private async void Webview_Loaded(object sender, RoutedEventArgs e)
        {
            await webview.EnsureCoreWebView2Async();
            jsEngine = new WebViewJsEngine(webview.CoreWebView2);
            _youTubeStreams = new YouTubeStreams(jsEngine);
            searchButton.IsEnabled = true;
        }

        private async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            var results = await _youTubeStreams.GetAllStreamsAsync(queryTextBox.Text, true);
            resultsListView.Items.Clear();

            var audioOnly = results.OfType<IAudioOnlyStreamInfo>().Select(stream => new StreamInfoVM
            {
                Bitrate = stream.Bitrate.ToString(),
                Codec = stream.Codec,
                Container = stream.Container,
                Label = stream.Bitrate.ToString(),
                Type = stream.StreamType,
                Url = stream.PlayableUrl.Url
            });
            var videos = results.OfType<IVideoStreamInfo>().Select(stream => new StreamInfoVM
            {
                Bitrate = stream.Bitrate.ToString(),
                Label = stream.QualityLabel,
                Codec = stream.Codec,
                Container = stream.Container,
                Type = stream.StreamType,
                Url = stream.PlayableUrl.Url
            });
            var streams = audioOnly.Concat(videos).OrderBy(x => x.Type).ThenBy(x => x.Container).ThenBy(x => x.Bitrate);
            foreach (var stream in streams)
            {
                resultsListView.Items.Add(stream);
            }
        }

        private void resultsListView_ItemClick(object sender, Microsoft.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            var vm = e.ClickedItem as StreamInfoVM;
            dataPackage.SetText(vm.Url);
            Clipboard.SetContent(dataPackage);
        }
    }
}
