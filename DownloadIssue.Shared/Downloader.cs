using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadIssue.Shared
{
    public class Downloader
    {
        private HttpClient _httpClient;

        public Downloader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DownloadAsync(string url, string path)
        {
            var contentLength = await GetContentLength(url);
            var progress = new Progress<double>((e) =>
            {
                var p = (int)(e / contentLength * 100);
                Console.WriteLine($"downloaded {p}%");
            });
            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            await DownloadRanges(url, contentLength, fileStream, progress).ConfigureAwait(false);

            //await DownloadUsingWebRequest(url, contentLength, fileStream);
        }

        private async Task DownloadRanges(string url, long contentLength, Stream destination,
            IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            var ranges = CreateRanges(contentLength, 1);
            foreach (var range in ranges)
            {
                //await DownloadRange(url, range, destination, progress, cancellationToken).ConfigureAwait(false);
                await DownloadRangeUsingWebRequest(url, range, destination, progress, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task DownloadRange(string url, (long, long) range, Stream destination,
            IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Version = HttpVersion.Version11;
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Host", new Uri(url).Host);
            var useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.5060.134 Safari/537.36 OPR/89.0.4447.101";
            //var useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36";
            request.Headers.Add("User-Agent", useragent);
            //request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            //request.Headers.Add("Accept-Language", "pl-PL,pl;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Headers.Add("Range", "bytes=0-");
            //request.Headers.Add("sec-fetch-mode", "no-cors");
            //request.Headers.Add("accept-encoding", "identity;q=1, *;q=0");
            //request.Headers.Add("Referer", "https://rr2---sn-u2oxu-3ufs.googlevideo.com/videoplayback?expire=1661299753&ei=yRcFY9mjBbrb7ASgjrjgAw&ip=2a00%3Af41%3A2c39%3Ac34%3A2db2%3Aa54c%3A3cb9%3A4db9&id=o-ADK0NW4IUZOmbz9QVOqRRB5FClNEEaWvGNbSg3udlo0y&itag=140&source=youtube&requiressl=yes&mh=g5&mm=31%2C29&mn=sn-u2oxu-3ufs%2Csn-u2oxu-f5fer&ms=au%2Crdu&mv=m&mvi=2&pcm2cms=yes&pl=48&gcr=pl&initcwndbps=585000&vprv=1&mime=audio%2Fmp4&ns=RXL1n2-g9Ehuy1J7K0skxmEH&gir=yes&clen=3762144&dur=232.385&lmt=1653307660360778&mt=1661277929&fvip=3&keepalive=yes&fexp=24001373%2C24007246&c=WEB&rbqsm=fr&txp=4532434&n=LiB6GHUhtqnaCQ&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Cgcr%2Cvprv%2Cmime%2Cns%2Cgir%2Cclen%2Cdur%2Clmt&lsparams=mh%2Cmm%2Cmn%2Cms%2Cmv%2Cmvi%2Cpcm2cms%2Cpl%2Cinitcwndbps&lsig=AG3C_xAwRgIhAI1i04mBnpy0J9SVZhTxdAVwqigJpwxH5PhPdQJJZBIRAiEAqixKECcoJz5h6coWb5B_eJQHu47eJC_QiX33AITCBzc%3D&sig=AOq0QJ8wRQIhAILdCE5p0tPgzhE3LvvqOXGT_GrBKDqKVtBuA-qsAZSaAiBEym-I55xyFMT6eiWlmqmDHD9HtmIeR73v0GJs55CygQ==");
            //request.Headers.Add("sec-ch-ua", "");
            //request.Headers.Add("sec-ch-ua-mobile", "?0");
            //request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
            //request.Headers.Add("sec-fetch-dest", "video");
            //request.Headers.Add("sec-fetch-mode", "no-cors");
            //request.Headers.Add("sec-fetch-site", "same-origin");

            //request.Headers.Range = new RangeHeaderValue(range.Item1, range.Item2);

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    await contentStream.CopyToAsync(destination, progress, range.Item1, cancellationToken);
                }
            }
        }

        private async Task DownloadRangeUsingWebRequest(string url, (long, long) range, Stream destination,
            IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.ProtocolVersion = HttpVersion.Version11;
            request.AddRange(range.Item1, range.Item2);
            request.Accept = "*/*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.5060.134 Safari/537.36 OPR/89.0.4447.101";

            using HttpWebResponse downloadResponse = request.GetResponse() as HttpWebResponse;
            if (downloadResponse.StatusCode == HttpStatusCode.OK ||
                downloadResponse.StatusCode == HttpStatusCode.PartialContent ||
                downloadResponse.StatusCode == HttpStatusCode.Created ||
                downloadResponse.StatusCode == HttpStatusCode.Accepted ||
                downloadResponse.StatusCode == HttpStatusCode.ResetContent)
            {
                using Stream responseStream = downloadResponse?.GetResponseStream();
                if (responseStream != null)
                {
                    await responseStream.CopyToAsync(destination, progress, range.Item1, cancellationToken);
                }
            }
            else
            {
                throw new Exception("HttpWebResponse error");
            }
        }

        private List<(long, long)> CreateRanges(long fileSize, long parts)
        {
            var start = 0;
            var ranges = new List<(long, long)>();
            long rangeSize = fileSize / parts;
            for (int i = 0; i < parts; i++)
            {
                long startPosition = start + (i * rangeSize);
                long endPosition = startPosition + rangeSize - 1;
                ranges.Add((startPosition, endPosition));
            }
            var lastRange = ranges.Last();
            lastRange.Item2 += fileSize % parts;
            ranges.RemoveAt((int)parts - 1);
            ranges.Add(lastRange);

            return ranges;
        }

        private async Task<long> GetContentLength(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Host", new Uri(url).Host);

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return response.Content.Headers.ContentLength ?? 0;
            }
            return 0;
        }

        private async Task DownloadUsingWebRequest(string url, long contentLength, Stream destination)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.AddRange(0, contentLength);
            request.Accept = "*/*";
            request.Method = "GET";
            request.ProtocolVersion = HttpVersion.Version11;
            var useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.5060.134 Safari/537.36 OPR/89.0.4447.101";
            request.UserAgent = useragent;
            using HttpWebResponse downloadResponse = request.GetResponse() as HttpWebResponse;
            if (downloadResponse.StatusCode == HttpStatusCode.OK ||
                downloadResponse.StatusCode == HttpStatusCode.PartialContent ||
                downloadResponse.StatusCode == HttpStatusCode.Created ||
                downloadResponse.StatusCode == HttpStatusCode.Accepted ||
                downloadResponse.StatusCode == HttpStatusCode.ResetContent)
            {
                using Stream responseStream = downloadResponse?.GetResponseStream();
                if (responseStream != null)
                {
                    await responseStream.CopyToAsync(destination);
                }
            }
            else
            {

            }
        }
    }
}
