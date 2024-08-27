using Shouldly;
using YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher;

namespace YouTubeStreamsExtractor.Tests
{
    internal class Tests
    {
        private HttpClient _httpClient;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _httpClient = new HttpClient();
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            _httpClient?.Dispose();
        }

        private IJavaScriptEngine GetJavaScriptEngine()
        {
            return new JavaScriptJintEngine();
            //return new JavaScriptJurassicEngine();
        }

        private async Task<string> DownloadPlayerCode(string url)
        {
            var player = await _httpClient.GetStringAsync(url);

            return player;
        }

        public static object[] sigCases =
        {
            new object[] { "https://www.youtube.com/s/player/6ed0d907/player_ias.vflset/en_US/base.js",
                "2aq0aqSyOoJXtK73m-uME_jv7-pT15gOFC02RFkGMqWpzEICs69VdbwQ0LDp1v7j8xx92efCJlYFYb1sUkkBSPOlPmXgIARw8JQ0qOAOAA",
                "AOq0QJ8wRAIgXmPlOPSBkkUs1bYFYlJCfe29xx8j7v1pDL2QwbdV96sCIEzpWqMGkFR20CFOg51Tp-7vj_EMu-m37KtXJoOySqa0" },
            new object[] { "https://s.ytimg.com/yts/jsbin/html5player-en_US-vflKjOTVq/html5player.js",
                "312AA52209E3623129A412D56A40F11CB0AF14AE.3EE09501CB14E3BCDC3B2AE808BF3F1D14E7FBF12",
                "112AA5220913623229A412D56A40F11CB0AF14AE.3EE0950FCB14EEBCDC3B2AE808BF331D14E7FBF3"}
        };

        public static object[] nCases =
        {
            new object[] {
                "https://www.youtube.com/s/player/7862ca1f/player_ias.vflset/en_US/base.js",
                "X_LCxVDjAavgE5t", "yxJ1dM6iz5ogUg",
            },
            new object[] {
                "https://www.youtube.com/s/player/9216d1f7/player_ias.vflset/en_US/base.js",
                "SLp9F5bwjAdhE9F-", "gWnb9IK2DJ8Q1w",
            },
            new object[] {
                "https://www.youtube.com/s/player/f8cb7a3b/player_ias.vflset/en_US/base.js",
                "oBo2h5euWy6osrUt", "ivXHpm7qJjJN",
            },
            new object[] {
                "https://www.youtube.com/s/player/2dfe380c/player_ias.vflset/en_US/base.js",
                "oBo2h5euWy6osrUt", "3DIBbn3qdQ",
            },
            new object[] {
                "https://www.youtube.com/s/player/f1ca6900/player_ias.vflset/en_US/base.js",
                "cu3wyu6LQn2hse", "jvxetvmlI9AN9Q",
            },
            new object[] {
                "https://www.youtube.com/s/player/8040e515/player_ias.vflset/en_US/base.js",
                "wvOFaY-yjgDuIEg5", "HkfBFDHmgw4rsw",
            },
            new object[] {
                "https://www.youtube.com/s/player/e06dea74/player_ias.vflset/en_US/base.js",
                "AiuodmaDDYw8d3y4bf", "ankd8eza2T6Qmw",
            },
            new object[] {
                "https://www.youtube.com/s/player/5dd88d1d/player-plasma-ias-phone-en_US.vflset/base.js",
                "kSxKFLeqzv_ZyHSAt", "n8gS8oRlHOxPFA",
            },
            new object[] {
                "https://www.youtube.com/s/player/324f67b9/player_ias.vflset/en_US/base.js",
                "xdftNy7dh9QGnhW", "22qLGxrmX8F1rA",
            },
            new object[] {
                "https://www.youtube.com/s/player/4c3f79c5/player_ias.vflset/en_US/base.js",
                "TDCstCG66tEAO5pR9o", "dbxNtZ14c-yWyw",
            },
            new object[] {
                "https://www.youtube.com/s/player/c81bbb4a/player_ias.vflset/en_US/base.js",
                "gre3EcLurNY2vqp94", "Z9DfGxWP115WTg",
            },
            new object[] {
                "https://www.youtube.com/s/player/1f7d5369/player_ias.vflset/en_US/base.js",
                "batNX7sYqIJdkJ", "IhOkL_zxbkOZBw",
            },
            new object[] {
                "https://www.youtube.com/s/player/009f1d77/player_ias.vflset/en_US/base.js",
                "5dwFHw8aFWQUQtffRq", "audescmLUzI3jw",
            },
            new object[] {
                "https://www.youtube.com/s/player/dc0c6770/player_ias.vflset/en_US/base.js",
                "5EHDMgYLV6HPGk_Mu-kk", "n9lUJLHbxUI0GQ",
            },
            new object[] {
                "https://www.youtube.com/s/player/113ca41c/player_ias.vflset/en_US/base.js",
                "cgYl-tlYkhjT7A", "hI7BBr2zUgcmMg",
            },
            new object[] {
                "https://www.youtube.com/s/player/c57c113c/player_ias.vflset/en_US/base.js",
                "M92UUMHa8PdvPd3wyM", "3hPqLJsiNZx7yA",
            },
            new object[] {
                "https://www.youtube.com/s/player/5a3b6271/player_ias.vflset/en_US/base.js",
                "B2j7f_UPT4rfje85Lu_e", "m5DmNymaGQ5RdQ",
            },
            new object[] {
                "https://www.youtube.com/s/player/7a062b77/player_ias.vflset/en_US/base.js",
                "NRcE3y3mVtm_cV-W", "VbsCYUATvqlt5w",
            },
            new object[] {
                "https://www.youtube.com/s/player/dac945fd/player_ias.vflset/en_US/base.js",
                "o8BkRxXhuYsBCWi6RplPdP", "3Lx32v_hmzTm6A",
            },
            new object[] {
                "https://www.youtube.com/s/player/6f20102c/player_ias.vflset/en_US/base.js",
                "lE8DhoDmKqnmJJ", "pJTTX6XyJP2BYw",
            },
            new object[] {
                "https://www.youtube.com/s/player/cfa9e7cb/player_ias.vflset/en_US/base.js",
                "aCi3iElgd2kq0bxVbQ", "QX1y8jGb2IbZ0w",
            },
            new object[] {
                "https://www.youtube.com/s/player/8c7583ff/player_ias.vflset/en_US/base.js",
                "1wWCVpRR96eAmMI87L", "KSkWAVv1ZQxC3A",
            },
            new object[] {
                "https://www.youtube.com/s/player/b7910ca8/player_ias.vflset/en_US/base.js",
                "_hXMCwMt9qE310D", "LoZMgkkofRMCZQ",
            },
            new object[] {
                "https://www.youtube.com/s/player/590f65a6/player_ias.vflset/en_US/base.js",
                "1tm7-g_A9zsI8_Lay_", "xI4Vem4Put_rOg",
            },
            new object[] {
                "https://www.youtube.com/s/player/b22ef6e7/player_ias.vflset/en_US/base.js",
                "b6HcntHGkvBLk_FRf", "kNPW6A7FyP2l8A",
            },
            new object[] {
                "https://www.youtube.com/s/player/3400486c/player_ias.vflset/en_US/base.js",
                "lL46g3XifCKUZn1Xfw", "z767lhet6V2Skl",
            },
            new object[] {
                "https://www.youtube.com/s/player/20dfca59/player_ias.vflset/en_US/base.js",
                "-fLCxedkAk4LUTK2", "O8kfRq1y1eyHGw",
            },
            new object[] {
                "https://www.youtube.com/s/player/b12cc44b/player_ias.vflset/en_US/base.js",
                "keLa5R2U00sR9SQK", "N1OGyujjEwMnLw",
            }
        };


        [TestCaseSource(nameof(sigCases))]
        [Test]
        public async Task TestSig(string playerUrl, string sig, string expected)
        {
            var playerCode = await DownloadPlayerCode(playerUrl);
            var dec = new Decryptor(GetJavaScriptEngine());
            var functionName = dec.ExtractSigFunctionName(playerCode);
            functionName.ShouldNotBeNullOrWhiteSpace();

            var functionCode = dec.ExtractSigFunctionCode(functionName, playerCode);
            var jsEngine = GetJavaScriptEngine();
            var decrypted = await jsEngine.ExecuteJSCodeAsync(functionCode, functionName, sig);

            decrypted.ShouldBe(expected);
        }

        [TestCaseSource(nameof(nCases))]
        [Test]
        public async Task TestN(string playerUrl, string encryptedN, string expected)
        {
            var playerCode = await DownloadPlayerCode(playerUrl);
            var dec = new Decryptor(GetJavaScriptEngine());
            var functionName = dec.ExtractNFunctionName(playerCode);
            functionName.ShouldNotBeNullOrWhiteSpace();

            var functionCode = dec.ExtractNFunctionCode(functionName, playerCode);
            var jsEngine = GetJavaScriptEngine();

            try
            {
                var decrypted = await jsEngine.ExecuteJSCodeAsync(functionCode, functionName, encryptedN);
                decrypted.ShouldBe(expected);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
