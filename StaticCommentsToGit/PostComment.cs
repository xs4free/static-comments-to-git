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
            var formContents = await FormContentsFactory.Create(req);
            var comment = CommentFactory.Create(formContents);
            var settings = SettingsFactory.Create(formContents.Options.Origin, log);

            var reCaptcha = new ReCaptchaService(settings.ReCaptchaSecretKey, log);
            var reCaptchaResponse = await reCaptcha.Validate(formContents.Options.Recaptcha.Token);

            var akismetService = new AkismetService(settings, log);
            var akismetResponse = await akismetService.IsSpam(req, comment, formContents);

            var gitHubService = new GitHubService(settings.GitHubOwner, settings.GitHubRepository, settings.GitHubBranch,
                settings.GitHubCommentPath, settings.GitHubToken);
            var knownCommenterResponse = await gitHubService.IsKnownCommenter(comment.Name, comment.Email);

            var analyzer = new ModerationAnalyzer(settings, log);
            var analysisReport = analyzer.NeedsModeration(comment, reCaptchaResponse, akismetResponse, knownCommenterResponse);

            await gitHubService.AddComment(comment, analysisReport, knownCommenterResponse);

            return new OkResult();
        }
    }
}
