using Microsoft.AspNetCore.Http;

namespace StaticCommentsToGit.Extensions
{
    static class HttpRequestExtensions
    {
        public static void AddCorsHeaders(this HttpRequest req)
        {
            if (req.Headers.ContainsKey("Origin"))
            {
                var response = req.HttpContext.Response;
                var origin = req.Headers["Origin"];

                response.Headers.Add("Access-Control-Allow-Credentials", "true");
                response.Headers.Add("Access-Control-Allow-Origin", origin);
                response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
            }
        }
    }
}
