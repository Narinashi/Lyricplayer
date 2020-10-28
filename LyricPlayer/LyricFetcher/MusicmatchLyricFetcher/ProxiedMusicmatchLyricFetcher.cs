using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LyricPlayer.LyricFetcher.MusicmatchLyricFetcher
{
    class ProxiedMusicmatchLyricFetcher : MusicmatchLyricFetcher
    {
        public ProxiedMusicmatchLyricFetcher(string token, string proxy) : base(token, proxy)
        {
            Handler.AllowAutoRedirect = true;
            Handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;
            Handler.CookieContainer = new System.Net.CookieContainer();
            Handler.MaxAutomaticRedirections = 5;
            Handler.UseProxy = false;
        }
        protected sealed override async Task<string> SendRequest(string address)
        {
            try
            {
                var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("form[url]",address),
                    new KeyValuePair<string, string>("form[dataCenter]","random"),
                    new KeyValuePair<string, string>("terms-agreed","1"),
                });

                var result = await Client.PostAsync("https://www.hidemyass-freeproxy.com/process/en-in", content);
                if (result.IsSuccessStatusCode)
                    return await result.Content.ReadAsStringAsync();

                return string.Empty;
            }
            catch { return string.Empty; }
        }

    }
}
