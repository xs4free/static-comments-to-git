# static-comments-to-git
Publish markdown comments (for a blog) to a GIT repository using Azure Functions 2.0.

## Special thanks to the following websites/blogposts:
- [Azure Functions Access-Control-Allow-Credentials with CORS](https://blogs.msdn.microsoft.com/benjaminperkins/2017/04/12/azure-functions-access-control-allow-credentials-with-cors/) by Benjamin Perkins
- [Akismet.NET](https://github.com/RRosier/Akismet.NET/tree/master/Rosier.Akismet.Net) by Ronald Rossier
- [Octokit - GitHub API Client Library for .NET](https://github.com/octokit/octokit.net)

#TODO
- [ ] Move literals to configuration file
- [ ] Replace expecthostname with correct value
- [ ] Define a better response
- [ ] Create PullRequest when reCaptcha-score below certain threshold
- [ ] Implement Akismet SPAM check
