using Shouldly;
using YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher;

namespace YouTubeStreamsExtractor.Tests
{
    internal class SignatureTests
    {
        private IJavaScriptEngine GetJavaScriptEngine()
        {
            return new JavaScriptJurassicEngine();
        }

        private string GetPlayerCodeFromFile(string playerId)
        {
            var playerCode = File.ReadAllText(@$"files/{playerId}.js");
            return playerCode;
        }

        [Test]
        public void test_ExtractSigFunctionName()
        {
            var playerCode = GetPlayerCodeFromFile("2fd212f2");
            var dec = new Decryptor(GetJavaScriptEngine());

            var name = dec.ExtractSigFunctionName(playerCode);

            name.ShouldBe("Yva");
        }

        [Test]
        public void test_ExtractSigFunctionCode()
        {
            var functionName = "Yva";
            var playerCode = GetPlayerCodeFromFile("2fd212f2");
            var expectedSigCode = File.ReadAllText(@"files/sigCode.txt");

            var dec = new Decryptor(GetJavaScriptEngine());
            var sigCode = dec.ExtractSigFunctionCode(functionName, playerCode);

            sigCode.ShouldBe(expectedSigCode);
        }

        [Test]
        public async Task test_DecryptSignature()
        {
            var sig = "===gocfsKygy46CdN_sJAtBkm50hOzvK-vkNatbldzs_clCQICYcVCtgibVk0wsL0TXQjRWTX_gjz6y8FMyF4EF_3NKVFgIQRw8JQ0qOAqOA";
            var expectedDecrypted = "AOq0QJ8wRQIgFVKN3_FE4FyMF8y6zjg_XTWRjQXT0Lsw0kVbigtCVcYCIQClc_szdlbtaNkv-KvzOh05mkBtAJs_NdC64ygyKsfcog==";
            var functionName = "Yva";
            var expectedSigCode = File.ReadAllText(@"files/sigCode.txt");
            var jsEngine = new JavaScriptJurassicEngine();

            var decrypted = await jsEngine.ExecuteJSCodeAsync(expectedSigCode, functionName, sig);

            decrypted.ShouldBe(expectedDecrypted);
        }

        [Test]
        public void test_ExtractSigFunctionName2()
        {
            var playerCode = File.ReadAllText(@"files/playerCode2.js");
            var dec = new Decryptor(GetJavaScriptEngine());

            var name = dec.ExtractSigFunctionName(playerCode);

            name.ShouldBe("$va");
        }

        [Test]
        public void test_ExtractSigFunctionCode2()
        {
            var functionName = "$va";
            var playerCode = File.ReadAllText(@"files/playerCode2.js");
            var expectedSigCode = File.ReadAllText(@"files/sigCode2.txt");

            var dec = new Decryptor(GetJavaScriptEngine());
            var sigCode = dec.ExtractSigFunctionCode(functionName, playerCode);

            sigCode.ShouldBe(expectedSigCode);
        }
    }
}
