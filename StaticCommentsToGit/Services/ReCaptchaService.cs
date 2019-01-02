using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace StaticCommentsToGit.Services
{
    public class ReCaptchaService
    {
        private readonly string _secretKey;
        private readonly ILogger _log;
        private static readonly HttpClient Client = new HttpClient();

        public ReCaptchaService(string secretKey, ILogger log)
        {
            _secretKey = secretKey;
            _log = log;
        }

        public async Task<RecaptchaResponse> Validate(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            string url = $"https://www.google.com/recaptcha/api/siteverify?secret={_secretKey}&response={token}";
            var result = await Client.PostAsync(url, null).ConfigureAwait(false);
            var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            _log.LogInformation("Received reCAPTCHA response: '{0}'", content);

            var recaptchaResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(content);

            return recaptchaResponse;
        }
    }
}
