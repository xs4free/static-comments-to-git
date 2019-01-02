using System;
using System.Threading.Tasks;
using Akismet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StaticCommentsToGit.Entities;

namespace StaticCommentsToGit.Services
{
    class AkismetService
    {
        private readonly Settings _settings;
        private readonly ILogger _log;

        public AkismetService(Settings settings, ILogger log)
        {
            _settings = settings;
            _log = log;
        }

        public async Task<AkismetResponse> IsSpam(HttpRequest req, Comment comment, FormContents formContents)
        {
            var userAgent = req.Headers["User-Agent"];
            var blog = new Uri(_settings.AkismetBlogUrl);

            using (var akismetClient = new AkismetClient(_settings.AkismetApiKey, blog, userAgent))
            {
                var akismetResult = await akismetClient.IsSpam2(new AkismetComment
                {
                    Blog = blog,
                    CommentAuthor = comment.Name,
                    CommentAuthorEmail = formContents.Fields.Email,
                    CommentContent = comment.Body,
                    CommentType = "comment", // https://akismet.com/development/api/#comment-check
                    Permalink = formContents.Options.Origin,
                    Referrer = req.Headers["Referer"],
                    UserAgent = userAgent,
                    UserIp = req.HttpContext.Connection.RemoteIpAddress.ToString()
                });

                var response = new AkismetResponse
                {
                    IsSpam = akismetResult.IsSpam,
                    Text = akismetResult.Text,
                    ProTip = akismetResult.ProTip,
                    DebugHelp = akismetResult.DebugHelp
                };

                _log.LogDebug("Akismet result: {0}", response);

                return response;
            }
        }
    }
}
