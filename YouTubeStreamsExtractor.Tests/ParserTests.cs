using Shouldly;
using YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher;

namespace YouTubeStreamsExtractor.Tests
{
    internal class ParserTests
    {
        private IJavaScriptEngine GetJavaScriptEngine()
        {
            return new JavaScriptJintEngine();
        }

        [Test]
        public void test_ParseQuery()
        {
            var signatureCipher = "s=M%3D%3DeiyDwr9xDcgsg4maVY3qpAAaonNofTBfjEJgoEObC9AEiAXzw8ENGRypA_NWYOdSxRamXl04_C09GYpJ20xt9jzNNAhIgRw8JQ0qOAqOA&sp=sig&url=https://rr2---sn-u2oxu-3ufs.googlevideo.com/videoplayback%3Fexpire%3D1660248357%26ei%3DxQz1YrvIH9qlyQXiopzQCg%26ip%3D2a01%253A111f%253A1003%253A5900%253Af03f%253A5343%253A780f%253Aad20%26id%3Do-AEjJ5NUSh4LBkzw9ZD-FItO1R2YNYnTN885rLNwy3vSS%26itag%3D136%26aitags%3D133%252C134%252C135%252C136%252C160%252C242%252C243%252C244%252C247%252C278%252C298%252C299%252C302%252C303%252C394%252C395%252C396%252C397%252C398%252C399%26source%3Dyoutube%26requiressl%3Dyes%26mh%3Dg5%26mm%3D31%252C29%26mn%3Dsn-u2oxu-3ufs%252Csn-u2oxu-f5fer%26ms%3Dau%252Crdu%26mv%3Dm%26mvi%3D2%26pcm2cms%3Dyes%26pl%3D36%26gcr%3Dpl%26initcwndbps%3D1215000%26spc%3DlT-Khh-i6XVqTrIJwkPuoH6TcEFbdYQ%26vprv%3D1%26mime%3Dvideo%252Fmp4%26ns%3Ddol4IPi1mcIEqqMbodHbpGcH%26gir%3Dyes%26clen%3D6739974%26dur%3D232.333%26lmt%3D1653309686871534%26mt%3D1660226451%26fvip%3D3%26keepalive%3Dyes%26fexp%3D24001373%252C24007246%26c%3DWEB%26rbqsm%3Dfr%26txp%3D4535434%26n%3DRQXCPwaoKTlr3HTc%26sparams%3Dexpire%252Cei%252Cip%252Cid%252Caitags%252Csource%252Crequiressl%252Cgcr%252Cspc%252Cvprv%252Cmime%252Cns%252Cgir%252Cclen%252Cdur%252Clmt%26lsparams%3Dmh%252Cmm%252Cmn%252Cms%252Cmv%252Cmvi%252Cpcm2cms%252Cpl%252Cinitcwndbps%26lsig%3DAG3C_xAwRAIgQaM3BZ43za6kqrrPlYKKHQsrUKgtIastksZ41QQrTKkCIEbCJ1dDlev2JXBfvo7BpsLWK8SyjgOl63zYLu0UkTE8";
            var dec = new Decryptor(GetJavaScriptEngine());
            var parsed = dec.ParseQuery(signatureCipher);

            parsed.ShouldContainKey("s");
            parsed.ShouldContainKey("url");
            parsed.ShouldContainKey("sp");
            parsed["s"].ShouldBe("M==eiyDwr9xDcgsg4maVY3qpAAaonNofTBfjEJgoEObC9AEiAXzw8ENGRypA_NWYOdSxRamXl04_C09GYpJ20xt9jzNNAhIgRw8JQ0qOAqOA");
        }

        [Test]
        public void test_GetPLayerUrl()
        {
            var webpage = "HA4CFG9zRpaCNjYj33SYjzQ9cTy\",\"PLAYER_JS_URL\":\"/s/player/2fd212f2/player_ias.vflset/en_US/base.js\",\"PLAYER_CSS_URL\":\"/s/player/2fd212f";

            var dec = new Decryptor(GetJavaScriptEngine());
            var playerUrl = dec.GetPlayerUrl(webpage);

            playerUrl.ShouldBe("https://www.youtube.com/s/player/2fd212f2/player_ias.vflset/en_US/base.js");
        }

        //[Test]
        //public async Task test_GetPlayerCode()
        //{
        //    var playerUrl = "https://www.youtube.com/s/player/2fd212f2/player_ias.vflset/en_US/base.js";

        //    var dec = new Decryptor(GetJavaScriptEngine());
        //    var code = await dec.GetPlayerCode(playerUrl);

        //    code.ShouldNotBeNullOrEmpty();
        //    code.ShouldContain("var _yt_player={}");
        //}

        [Test]
        public void test_GetPlayerId()
        {
            var playerUrl = "https://www.youtube.com/s/player/2fd212f2/player_ias.vflset/en_US/base.js";

            var dec = new Decryptor(GetJavaScriptEngine());
            var playerId = dec.GetPlayerId(playerUrl);

            playerId.ShouldBe("2fd212f2");
        }
    }
}
