[![Build status](https://dev.azure.com/xs4free/Progz.nl%20home-automation%20blog/_apis/build/status/Static-comments-to-Git)](https://dev.azure.com/xs4free/Progz.nl%20home-automation%20blog/_build/latest?definitionId=4)

# Description
Static-comments-to-git enables you to publish markdown comments (for a blog) to a GIT repository using Azure Functions 2.0.

# Settings required to run the function
This Azure function requires the following settings to run (for local development add them to `local.settings.json`):

|Name                 |Example value                               |Remark                                                                                                                              |
|---------------------|--------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------|
|`ReCaptchaSecretKey` | `6Lc3GHwUAAAAAGQyJylDj6GfdeGnlEvD3HDKb8YR` | reCAPTCHA v3 secret key. Create keys using [Google ReCaptcha Admin](https://www.google.com/recaptcha/admin) |
|`ReCaptchaHostname`  | `localhost` or `progz.nl`                  | Expected hostname where Captcha was generated. |
|`ReCaptchaAction`    | `postcomment`                              | Expected action that was included when Captcha was generated. |
|`GitHubOwner`        | `xs4free`								   | Name of the GitHub user that will commit comments to the repository. |
|`GitHubRepository`   | `static-comments-to-git-publish-test`      | Name of the GitHub repository where comments will be committed. |
|`GitHubBranch`       | `master`                                   | Name of the Git branch where comments will be committed. |
|`GitHubToken`        | `668a654979d129ce3d6115bad80c511139ddb243` | GitHub Personal Access Token used to authenticate. Create on [GitHub Developer Settings page](https://github.com/settings/tokens) with `public_repo` scope. |

ps. Example values above won't work, since both ReCaptcha secret and GitHubToken are not valid (any more).

# local.settings.json
For local development allow all sites to do cross-site requests by enabling the `CORS`-setting.
An example contents for the `local.settings.json` file could be:
```
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ReCaptchaSecretKey": "6Lc3GHwUAAAAAGQyJylDj6GfdeGnlEvD3HDKb8YR",
    "ReCaptchaHostname": "localhost",
    "ReCaptchaAction": "postcomment",
    "GitHubOwner": "xs4free",
    "GitHubRepository": "static-comments-to-git-publish-test",
    "GitHubBranch": "master",
    "GitHubToken": "668a654979d129ce3d6115bad80c511139ddb243"
  },
  "Host": {
    "LocalHttpPort": 7071,
    "CORS": "*"
  }
}
```

## Special thanks to the following websites/blogposts:
- [Azure Functions Access-Control-Allow-Credentials with CORS](https://blogs.msdn.microsoft.com/benjaminperkins/2017/04/12/azure-functions-access-control-allow-credentials-with-cors/) by Benjamin Perkins
- [Akismet.NET](https://github.com/RRosier/Akismet.NET/tree/master/Rosier.Akismet.Net) by Ronald Rossier
- [Octokit - GitHub API Client Library for .NET](https://github.com/octokit/octokit.net)

#TODO
- [ ] Define a better response
- [ ] Create PullRequest when reCaptcha-score below certain threshold
- [ ] Implement Akismet SPAM check
- [ ] Split code into seperate library and add unittesting
- [ ] Implement [unit-testing for Azure Function](https://docs.microsoft.com/nl-nl/azure/azure-functions/functions-test-a-function)
- [x] ~~Move literals to configuration file~~
- [x] ~~Create Azure DevOps CI/CD (and add badge to readme.md)~~
