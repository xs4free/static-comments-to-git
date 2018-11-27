using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Octokit;

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

            AddCorsHeaders(req);

            var collection = await req.ReadFormAsync();

            ReCaptcha reCaptcha = new ReCaptcha("<YOUR_RECAPTCHA_SITESECRET>");
            string token = collection["options[reCaptcha][token]"];

            bool valid = await reCaptcha.Validate(token, "localhost"); //"http://www.progz.nl"

            if (valid)
            {
                //var config = new ConfigurationBuilder()
                //    .SetBasePath(context.FunctionAppDirectory)
                //    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                //    .AddEnvironmentVariables()
                //    .Build();

                string name = req.Query["name"];
                name = name ?? collection["fields[name]"];

                var github = new GitHubClient(new ProductHeaderValue("StaticCommentsToGit"));
                github.Credentials =
                    new Credentials("<YOUR_GITHUB_ACCESS_TOKEN>"); //https://github.com/settings/tokens

                // github variables
                var owner = "xs4free";
                var repo = "static-comments-to-git-publish-test";
                var path = $"{name}.{DateTime.Now.Ticks}.txt";
                var branch = "master";
                var createFileRequest = new CreateFileRequest("First commit message", "File contents", branch);

                await github.Repository.Content.CreateFile(owner, repo, path, createFileRequest);

                return new OkObjectResult($"Hello, {name}. reCaptcha valid");
            }

            return new BadRequestObjectResult("reCaptcha invalid");
        }

        private static void AddCorsHeaders(HttpRequest req)
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
