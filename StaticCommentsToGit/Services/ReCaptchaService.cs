using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StaticCommentsToGit.Services
{
    public class ReCaptchaService
    {
        private readonly string _secretKey;
        private static readonly HttpClient Client = new HttpClient();

        public ReCaptchaService(string secretKey)
        {
            _secretKey = secretKey;
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
            var recaptchaResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(content);

            return recaptchaResponse;
        }
    }
}
