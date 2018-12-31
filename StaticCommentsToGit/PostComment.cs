using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StaticCommentsToGit.Extensions;
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

            req.AddCorsHeaders();
            var formContents = await FormContentsFactory.Create(req);

            ReCaptcha reCaptcha = new ReCaptcha("6Lc3GHwUAAAAAGQyJylDj6GfdeGnlEvD3HDKb8YR");
            bool valid = await reCaptcha.Validate(formContents.Options.Recaptcha.Token, "localhost", "postcomment"); //"http://www.progz.nl"

            if (valid)
            {
                //var config = new ConfigurationBuilder()
                //    .SetBasePath(context.FunctionAppDirectory)
                //    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                //    .AddEnvironmentVariables()
                //    .Build();

                var comment = CommentFactory.Create(formContents.Fields);

                var gitHub = new GitHubService(
                    "xs4free", "static-comments-to-git-publish-test", "master",
                    "668a654979d129ce3d6115bad80c511139ddb243");
                gitHub.AddComment(comment);

                return new OkObjectResult($"Hello, {comment.Name}. reCaptcha valid");
            }

            return new BadRequestObjectResult("reCaptcha invalid");
        }
    }
}
