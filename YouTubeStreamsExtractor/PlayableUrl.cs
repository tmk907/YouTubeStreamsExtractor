namespace YouTubeStreamsExtractor
{
    public class PlayableUrl
    {
        private readonly string _signatureCipher;
        private readonly string _playerUrl;
        private bool isUrlDecrypted = false;

        public string Url { get; private set; } = "";

        public PlayableUrl(string signatureCipher, string playerUrl)
        {
            _signatureCipher = signatureCipher;
            _playerUrl = playerUrl;
        }

        public async Task<string> PrepareAsync(string? rawUrl, IDecryptor decryptor)
        {
            if (!isUrlDecrypted)
            {
                if (rawUrl == null)
                {
                    Url = await decryptor.GetStreamUrl(_signatureCipher, _playerUrl);
                }
                else
                {
                    Url = await decryptor.ReplaceNWithDecrypted(rawUrl, _playerUrl);
                }
                isUrlDecrypted = true;
            }
            return Url;
        }
    }
}
