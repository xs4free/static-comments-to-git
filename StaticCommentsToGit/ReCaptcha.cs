using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StaticCommentsToGit
{
    public class ReCaptcha
    {
        private readonly string _siteSecret;
        private static readonly HttpClient Client = new HttpClient();

        public ReCaptcha(string siteSecret)
        {
            _siteSecret = siteSecret;
        }

        public async Task<bool> Validate(string token, string expectedHostname, string expectedAction)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            string url = $"https://www.google.com/recaptcha/api/siteverify?secret={_siteSecret}&response={token}";
            var result = await Client.PostAsync(url, null).ConfigureAwait(false);
            var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            var recaptchaResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(content);

            return recaptchaResponse.success 
                   && string.Compare(recaptchaResponse.hostname, expectedHostname, StringComparison.InvariantCultureIgnoreCase) == 0
                   && string.Compare(recaptchaResponse.action, expectedAction, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
