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
|`AkismetApiKey`      | `aaa111aaa111`                             | Akismet API KEY. Can be found on [Akismet Account overview](https://akismet.com/account/) |
|`AkismetBlogUrl`     | `https://www.progz.nl`                     | The front page or home URL of the blog where the comment will be hosted (must be a full URI).
|`GitHubOwner`        | `xs4free`								   | Name of the GitHub user that will commit comments to the repository. |
|`GitHubRepository`   | `static-comments-to-git-publish-test`      | Name of the GitHub repository where comments will be committed. |
|`GitHubBranch`       | `master`                                   | Name of the Git branch where comments will be committed. |
|`GitHubCommentPath`  | `data\comments`                            | Base-path in the GitHub repository where comments are stored. |
|`GitHubToken`        | `668a654979d129ce3d6115bad80c511139ddb243` | GitHub Personal Access Token used to authenticate. Create one at the [GitHub Developer Settings page](https://github.com/settings/tokens) with `public_repo` scope. |
|`SiteNames`          | `travel,homeautomation`                    | [Optional] Comma-separated list of site-names comments will be processed for. These names are only used inside StaticCommentsToGit. |
|`SiteOrigins`        | `/blog,/homeautomation`                    | [Optional] Comma-separated list of texts that are part of the origin for a comment. Number of values should equal number of `siteNames` |

ps. Example values above won't work, since the ReCaptchaSecretKey, AkistmetApiKey and GitHubToken are not valid (any more).

# local.settings.json for one site
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
    "AkismetApiKey": "aaa111aaa111",
    "AkismetBlogUrl": "https://www.progz.nl" ,
    "GitHubOwner": "xs4free",
    "GitHubRepository": "static-comments-to-git-publish-test",
    "GitHubBranch": "master",
    "GitHubCommentPath": "data\\comments",
    "GitHubToken": "668a654979d129ce3d6115bad80c511139ddb243"
  },
  "Host": {
    "LocalHttpPort": 7071,
    "CORS": "*"
  }
}
```

# local.settings.json for multiple sites
An example contents for the `local.settings.json` file could be:
```
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
	"SiteNames": "travel,homeautomation",
	"SiteOrigins": "/blog,/homeautomation"
    "ReCaptchaSecretKey": "6Lc3GHwUAAAAAGQyJylDj6GfdeGnlEvD3HDKb8YR",
    "ReCaptchaHostname": "localhost",
    "ReCaptchaAction": "postcomment",
    "AkismetApiKey": "aaa111aaa111",
    "AkismetBlogUrl": "https://www.progz.nl" ,
    "GitHubOwner": "xs4free",
    "travel-GitHubRepository": "travelRepo",
    "homeautomation-GitHubRepository": "homeautomationRepo",
    "GitHubBranch": "master",
    "GitHubCommentPath": "data\\comments",
    "GitHubToken": "668a654979d129ce3d6115bad80c511139ddb243",
  },
  "Host": {
    "LocalHttpPort": 7071,
    "CORS": "*"
  }
}
```


## Special thanks to the following websites/blogposts:
- [Azure Functions Access-Control-Allow-Credentials with CORS](https://blogs.msdn.microsoft.com/benjaminperkins/2017/04/12/azure-functions-access-control-allow-credentials-with-cors/) by Benjamin Perkins
- [Akismet](https://www.nuget.org/packages/Akismet/) by Jason Nelson
- [Octokit - GitHub API Client Library for .NET](https://github.com/octokit/octokit.net)

#TODO
- [ ] Implement user-already-has-allowed-comment check by getting/creating/modifying a `known-commenters.json` in the GitHub repo
- [ ] Split code into seperate library and add unittesting
- [ ] Implement [unit-testing for Azure Function](https://docs.microsoft.com/nl-nl/azure/azure-functions/functions-test-a-function)
- [x] ~~Move literals to configuration file~~
- [x] ~~Create Azure DevOps CI/CD (and add badge to readme.md)~~
- [x] ~~Create PullRequest when reCaptcha-score below certain threshold~~
- [x] ~~Implement Akismet SPAM check~~
- [x] ~~Define a better response~~
- [x] ~~Add multi-site support by (1) adding comma-seperated sites-key to config and (2) adding site-name postfix to all site-keys with (3) a fallback to no-postfix for settings across sites~~
