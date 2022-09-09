using DownloadIssue.Shared;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DownloadIssueApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //var urlToDownload = "https://freetestdata.com/wp-content/uploads/2022/02/Free_Test_Data_10MB_MP4.mp4";
            var urlToDownload = "https://rr4---sn-u2oxu-3ufz.googlevideo.com/videoplayback?expire=1661384450&ei=omIGY7WUHsPR7ATOqqHoBQ&ip=2a01%3A111f%3A1003%3A5900%3A47c%3Aeddc%3A7ea1%3A76d8&id=o-ADNGu70x95ekFFCWxbOoJmSA9c9Y0ERgMJetbWzoBUub&itag=251&source=youtube&requiressl=yes&mh=g5&mm=31%2C29&mn=sn-u2oxu-3ufz%2Csn-u2oxu-f5fer&ms=au%2Crdu&mv=m&mvi=4&pcm2cms=yes&pl=47&gcr=pl&initcwndbps=861250&vprv=1&mime=audio%2Fwebm&ns=3rZLdkU7Wa-cUp7ISS3DxwQH&gir=yes&clen=3822436&dur=232.361&lmt=1653307657229968&mt=1661362388&fvip=3&keepalive=yes&fexp=24001373%2C24007246&c=WEB&rbqsm=fr&txp=4532434&n=tiqy39PntLm0Bw&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Cgcr%2Cvprv%2Cmime%2Cns%2Cgir%2Cclen%2Cdur%2Clmt&lsparams=mh%2Cmm%2Cmn%2Cms%2Cmv%2Cmvi%2Cpcm2cms%2Cpl%2Cinitcwndbps&lsig=AG3C_xAwRgIhAO1IEdYV6uMyZhkHcpMVUllD7byaCL_ftfinKaK6HGaPAiEAjZBoghA2YRQY12vytCY--6wTeTho_2lFaKi6LSQK0xY%3D&sig=AOq0QJ8wRQIhALIBaZuPgvXXsc7cJ0cQYxu63k6gEO7QH_LqyxbHtG5qAiBwVdTV9_25hhq6GupVLw2DM1fXegy8wVLJpT4h5pct4w==";

            var path = @"D:\YoutubeBackup\framework.mp4";

            var messageHandler = new HttpClientHandler();
            messageHandler.UseProxy = false;
            var httpClient = new HttpClient(messageHandler);

            var d = new Downloader(httpClient);
            await d.DownloadAsync(urlToDownload, path);

            Console.WriteLine("Done");
        }
    }
}
