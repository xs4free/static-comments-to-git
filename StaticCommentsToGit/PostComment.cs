using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StaticCommentsToGit.Factories;
using StaticCommentsToGit.Services;

namespace StaticCommentsToGit
{
    public static class PostComment
    {
        [FunctionName("PostComment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var formContents = await FormContentsFactory.Create(req);
            var settings = SettingsFactory.Create();

            var reCaptcha = new ReCaptchaService(settings.ReCaptchaSecretKey);
            bool valid = await reCaptcha.Validate(formContents.Options.Recaptcha.Token, settings.ReCaptchaHostname,
                settings.ReCaptchaAction);

            if (valid)
            {
                var comment = CommentFactory.Create(formContents.Fields);

                var gitHub = new GitHubService(settings.GitHubOwner, settings.GitHubRepository, settings.GitHubBranch,
                    settings.GitHubToken);

                gitHub.AddComment(comment);

                return new OkObjectResult($"Hello, {comment.Name}. reCaptcha valid");
            }

            return new BadRequestObjectResult("reCaptcha invalid");
        }

    }
}
