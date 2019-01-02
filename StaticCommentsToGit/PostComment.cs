using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StaticCommentsToGit.Analyzers;
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

            var settings = SettingsFactory.Create();
            var formContents = await FormContentsFactory.Create(req);
            var comment = CommentFactory.Create(formContents);

            var reCaptcha = new ReCaptchaService(settings.ReCaptchaSecretKey);
            var reCaptchaResponse = await reCaptcha.Validate(formContents.Options.Recaptcha.Token);

            var analyzer = new ModerationAnalyzer(settings);
            bool needsModeration = analyzer.NeedsModeration(comment, reCaptchaResponse);

            var gitHub = new GitHubService(settings.GitHubOwner, settings.GitHubRepository, settings.GitHubBranch,
                settings.GitHubCommentPath, settings.GitHubToken);

            await gitHub.AddComment(comment, needsModeration);

            return new OkObjectResult($"Hello, {comment.Name}. reCaptcha valid");
        }
    }
}
