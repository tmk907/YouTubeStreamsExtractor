using Shouldly;
using YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher;

namespace YouTubeStreamsExtractor.Tests
{
    internal class StreamsTests
    {
        private IJavaScriptEngine GetJavaScriptEngine()
        {
            return new JavaScriptJintEngine();
        }

        private string GetPlayerCodeFromFile(string playerId)
        {
            var playerCode = File.ReadAllText(@$"files/{playerId}.js");
            return playerCode;
        }

        [Test]
        public async Task test_GetStreamUrl()
        {
            var playerUrl = "https://www.youtube.com/s/player/2fd212f2/player_ias.vflset/en_US/base.js";
            var playerCode = GetPlayerCodeFromFile("2fd212f2");
            var signatureCipher = "s=M%3D%3DeiyDwr9xDcgsg4maVY3qpAAaonNofTBfjEJgoEObC9AEiAXzw8ENGRypA_NWYOdSxRamXl04_C09GYpJ20xt9jzNNAhIgRw8JQ0qOAqOA&sp=sig&url=https://rr2---sn-u2oxu-3ufs.googlevideo.com/videoplayback%3Fexpire%3D1660248357%26ei%3DxQz1YrvIH9qlyQXiopzQCg%26ip%3D2a01%253A111f%253A1003%253A5900%253Af03f%253A5343%253A780f%253Aad20%26id%3Do-AEjJ5NUSh4LBkzw9ZD-FItO1R2YNYnTN885rLNwy3vSS%26itag%3D136%26aitags%3D133%252C134%252C135%252C136%252C160%252C242%252C243%252C244%252C247%252C278%252C298%252C299%252C302%252C303%252C394%252C395%252C396%252C397%252C398%252C399%26source%3Dyoutube%26requiressl%3Dyes%26mh%3Dg5%26mm%3D31%252C29%26mn%3Dsn-u2oxu-3ufs%252Csn-u2oxu-f5fer%26ms%3Dau%252Crdu%26mv%3Dm%26mvi%3D2%26pcm2cms%3Dyes%26pl%3D36%26gcr%3Dpl%26initcwndbps%3D1215000%26spc%3DlT-Khh-i6XVqTrIJwkPuoH6TcEFbdYQ%26vprv%3D1%26mime%3Dvideo%252Fmp4%26ns%3Ddol4IPi1mcIEqqMbodHbpGcH%26gir%3Dyes%26clen%3D6739974%26dur%3D232.333%26lmt%3D1653309686871534%26mt%3D1660226451%26fvip%3D3%26keepalive%3Dyes%26fexp%3D24001373%252C24007246%26c%3DWEB%26rbqsm%3Dfr%26txp%3D4535434%26n%3DRQXCPwaoKTlr3HTc%26sparams%3Dexpire%252Cei%252Cip%252Cid%252Caitags%252Csource%252Crequiressl%252Cgcr%252Cspc%252Cvprv%252Cmime%252Cns%252Cgir%252Cclen%252Cdur%252Clmt%26lsparams%3Dmh%252Cmm%252Cmn%252Cms%252Cmv%252Cmvi%252Cpcm2cms%252Cpl%252Cinitcwndbps%26lsig%3DAG3C_xAwRAIgQaM3BZ43za6kqrrPlYKKHQsrUKgtIastksZ41QQrTKkCIEbCJ1dDlev2JXBfvo7BpsLWK8SyjgOl63zYLu0UkTE8";
            var expectedUrl = "https://rr2---sn-u2oxu-3ufs.googlevideo.com/videoplayback?expire=1660248357&ei=xQz1YrvIH9qlyQXiopzQCg&ip=2a01%3A111f%3A1003%3A5900%3Af03f%3A5343%3A780f%3Aad20&id=o-AEjJ5NUSh4LBkzw9ZD-FItO1R2YNYnTN885rLNwy3vSS&itag=136&aitags=133%2C134%2C135%2C136%2C160%2C242%2C243%2C244%2C247%2C278%2C298%2C299%2C302%2C303%2C394%2C395%2C396%2C397%2C398%2C399&source=youtube&requiressl=yes&mh=g5&mm=31%2C29&mn=sn-u2oxu-3ufs%2Csn-u2oxu-f5fer&ms=au%2Crdu&mv=m&mvi=2&pcm2cms=yes&pl=36&gcr=pl&initcwndbps=1215000&spc=lT-Khh-i6XVqTrIJwkPuoH6TcEFbdYQ&vprv=1&mime=video%2Fmp4&ns=dol4IPi1mcIEqqMbodHbpGcH&gir=yes&clen=6739974&dur=232.333&lmt=1653309686871534&mt=1660226451&fvip=3&keepalive=yes&fexp=24001373%2C24007246&c=WEB&rbqsm=fr&txp=4535434&n=5m24CCY_pjm2Kw&sparams=expire%2Cei%2Cip%2Cid%2Caitags%2Csource%2Crequiressl%2Cgcr%2Cspc%2Cvprv%2Cmime%2Cns%2Cgir%2Cclen%2Cdur%2Clmt&lsparams=mh%2Cmm%2Cmn%2Cms%2Cmv%2Cmvi%2Cpcm2cms%2Cpl%2Cinitcwndbps&lsig=AG3C_xAwRAIgQaM3BZ43za6kqrrPlYKKHQsrUKgtIastksZ41QQrTKkCIEbCJ1dDlev2JXBfvo7BpsLWK8SyjgOl63zYLu0UkTE8&sig=AOq0QJ8wRgIhANNzj9tx02JpYG90C_40lXmaRxSdOYWN_ApyRGNE8wzXAiEA9CbOEogJEjfBTfoNnoaAApq3YVam4gsgcDx9rwDyieM=";

            var dec = new Decryptor(GetJavaScriptEngine());
            var url = await dec.GetStreamUrl(signatureCipher, playerUrl, playerCode);

            url.ShouldBe(expectedUrl);
        }

        [Test]
        public async Task test_StreamUrl_cache()
        {
            var playerUrl = "https://www.youtube.com/s/player/2fd212f2/player_ias.vflset/en_US/base.js";
            var playerCode = GetPlayerCodeFromFile("2fd212f2");
            var signatureCipher = "s=M%3D%3DeiyDwr9xDcgsg4maVY3qpAAaonNofTBfjEJgoEObC9AEiAXzw8ENGRypA_NWYOdSxRamXl04_C09GYpJ20xt9jzNNAhIgRw8JQ0qOAqOA&sp=sig&url=https://rr2---sn-u2oxu-3ufs.googlevideo.com/videoplayback%3Fexpire%3D1660248357%26ei%3DxQz1YrvIH9qlyQXiopzQCg%26ip%3D2a01%253A111f%253A1003%253A5900%253Af03f%253A5343%253A780f%253Aad20%26id%3Do-AEjJ5NUSh4LBkzw9ZD-FItO1R2YNYnTN885rLNwy3vSS%26itag%3D136%26aitags%3D133%252C134%252C135%252C136%252C160%252C242%252C243%252C244%252C247%252C278%252C298%252C299%252C302%252C303%252C394%252C395%252C396%252C397%252C398%252C399%26source%3Dyoutube%26requiressl%3Dyes%26mh%3Dg5%26mm%3D31%252C29%26mn%3Dsn-u2oxu-3ufs%252Csn-u2oxu-f5fer%26ms%3Dau%252Crdu%26mv%3Dm%26mvi%3D2%26pcm2cms%3Dyes%26pl%3D36%26gcr%3Dpl%26initcwndbps%3D1215000%26spc%3DlT-Khh-i6XVqTrIJwkPuoH6TcEFbdYQ%26vprv%3D1%26mime%3Dvideo%252Fmp4%26ns%3Ddol4IPi1mcIEqqMbodHbpGcH%26gir%3Dyes%26clen%3D6739974%26dur%3D232.333%26lmt%3D1653309686871534%26mt%3D1660226451%26fvip%3D3%26keepalive%3Dyes%26fexp%3D24001373%252C24007246%26c%3DWEB%26rbqsm%3Dfr%26txp%3D4535434%26n%3DRQXCPwaoKTlr3HTc%26sparams%3Dexpire%252Cei%252Cip%252Cid%252Caitags%252Csource%252Crequiressl%252Cgcr%252Cspc%252Cvprv%252Cmime%252Cns%252Cgir%252Cclen%252Cdur%252Clmt%26lsparams%3Dmh%252Cmm%252Cmn%252Cms%252Cmv%252Cmvi%252Cpcm2cms%252Cpl%252Cinitcwndbps%26lsig%3DAG3C_xAwRAIgQaM3BZ43za6kqrrPlYKKHQsrUKgtIastksZ41QQrTKkCIEbCJ1dDlev2JXBfvo7BpsLWK8SyjgOl63zYLu0UkTE8";
            var expectedUrl = "https://rr2---sn-u2oxu-3ufs.googlevideo.com/videoplayback?expire=1660248357&ei=xQz1YrvIH9qlyQXiopzQCg&ip=2a01%3A111f%3A1003%3A5900%3Af03f%3A5343%3A780f%3Aad20&id=o-AEjJ5NUSh4LBkzw9ZD-FItO1R2YNYnTN885rLNwy3vSS&itag=136&aitags=133%2C134%2C135%2C136%2C160%2C242%2C243%2C244%2C247%2C278%2C298%2C299%2C302%2C303%2C394%2C395%2C396%2C397%2C398%2C399&source=youtube&requiressl=yes&mh=g5&mm=31%2C29&mn=sn-u2oxu-3ufs%2Csn-u2oxu-f5fer&ms=au%2Crdu&mv=m&mvi=2&pcm2cms=yes&pl=36&gcr=pl&initcwndbps=1215000&spc=lT-Khh-i6XVqTrIJwkPuoH6TcEFbdYQ&vprv=1&mime=video%2Fmp4&ns=dol4IPi1mcIEqqMbodHbpGcH&gir=yes&clen=6739974&dur=232.333&lmt=1653309686871534&mt=1660226451&fvip=3&keepalive=yes&fexp=24001373%2C24007246&c=WEB&rbqsm=fr&txp=4535434&n=5m24CCY_pjm2Kw&sparams=expire%2Cei%2Cip%2Cid%2Caitags%2Csource%2Crequiressl%2Cgcr%2Cspc%2Cvprv%2Cmime%2Cns%2Cgir%2Cclen%2Cdur%2Clmt&lsparams=mh%2Cmm%2Cmn%2Cms%2Cmv%2Cmvi%2Cpcm2cms%2Cpl%2Cinitcwndbps&lsig=AG3C_xAwRAIgQaM3BZ43za6kqrrPlYKKHQsrUKgtIastksZ41QQrTKkCIEbCJ1dDlev2JXBfvo7BpsLWK8SyjgOl63zYLu0UkTE8&sig=AOq0QJ8wRgIhANNzj9tx02JpYG90C_40lXmaRxSdOYWN_ApyRGNE8wzXAiEA9CbOEogJEjfBTfoNnoaAApq3YVam4gsgcDx9rwDyieM=";

            var dec = new Decryptor(GetJavaScriptEngine());
            var url = await dec.GetStreamUrl(signatureCipher, playerUrl, playerCode);
            var url2 = await dec.GetStreamUrl(signatureCipher, playerUrl, playerCode);

            url.ShouldBe(expectedUrl);
            url2.ShouldBe(expectedUrl);
        }
    }
}
