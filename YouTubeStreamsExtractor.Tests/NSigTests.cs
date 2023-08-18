using Shouldly;
using YouTubeStreamsExtractor.JsEngine.JavaScriptEngineSwitcher;

namespace YouTubeStreamsExtractor.Tests
{
    public class NSigTests
    {
        private IJavaScriptEngine GetJavaScriptEngine()
        {
            return new JavaScriptJintEngine();
            //return new JavaScriptJurassicEngine();
            //return new JavaScriptNiLEngine();
        }

        private string GetPlayerCodeFromFile(string playerId)
        {
            var playerCode = File.ReadAllText(@$"files/{playerId}.js");
            return playerCode;
        }

        [Test]
        [TestCase("2fd212f2", "Zka")]
        [TestCase("009f1d77", "Wka")]
        [TestCase("b7910ca8", "mma")]
        public void test_ExtractNFunctionName(string playerId, string expectedFunctionName)
        {
            var playerCode = GetPlayerCodeFromFile(playerId);
            var dec = new Decryptor(GetJavaScriptEngine());

            var name = dec.ExtractNFunctionName(playerCode);

            name.ShouldBe(expectedFunctionName);
        }

        [Test]
        [TestCase("2fd212f2", "Zka")]
        [TestCase("009f1d77", "Wka")]
        [TestCase("b7910ca8", "mma")]
        public void test_ExtractNFunctionCode(string playerId, string functionName)
        {
            var playerCode = GetPlayerCodeFromFile(playerId);
            var dec = new Decryptor(GetJavaScriptEngine());

            var functionCode = dec.ExtractNFunctionCode(functionName, playerCode);

            functionCode.ShouldNotBeNullOrEmpty();
            functionCode.ShouldStartWith(functionName);
            functionCode.ShouldContain("enhanced_except");
            functionCode.ShouldEndWith("return b.join(\"\")}");
        }

        [Test]
        [TestCase("2fd212f2", "RQXCPwaoKTlr3HTc", "5m24CCY_pjm2Kw")]
        [TestCase("009f1d77", "5dwFHw8aFWQUQtffRq", "audescmLUzI3jw")]
        [TestCase("b7910ca8", "_hXMCwMt9qE310D", "LoZMgkkofRMCZQ")]
        public async Task test_DecryptN(string playerId, string nSig, string expectedDecryptedNSig)
        {
            var playerCode = GetPlayerCodeFromFile(playerId);

            var dec = new Decryptor(GetJavaScriptEngine());
            var decryptedNSig = await dec.DecryptN(nSig, "", playerCode);

            decryptedNSig.ShouldBe(expectedDecryptedNSig);
        }

        // decrypt nSig using known function name and nSig
        [Test]
        [TestCase("2fd212f2")]
        public async Task test_ExecuteNFunctionCode(string playerId)
        {
            var functionName = "Zka";
            var nSig = "RQXCPwaoKTlr3HTc";
            var expectedDecryptedNSig = "5m24CCY_pjm2Kw";
            var playerCode = GetPlayerCodeFromFile(playerId);
            var jsEngine = new JavaScriptJurassicEngine();

            var dec = new Decryptor(jsEngine);
            var functionCode = dec.ExtractNFunctionCode(functionName, playerCode);
            var decryptedNSig = await jsEngine.ExecuteJSCodeAsync(functionCode, functionName, nSig);

            decryptedNSig.ShouldBe(expectedDecryptedNSig);
        }
    }
}