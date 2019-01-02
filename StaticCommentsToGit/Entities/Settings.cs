namespace StaticCommentsToGit.Entities
{
    class Settings
    {
        public string ReCaptchaSecretKey { get; set; }
        public string ReCaptchaHostname { get; set; }
        public string ReCaptchaAction { get; set; }
        public decimal ReCaptchaMinimumScore { get; set; }
        public string AkismetApiKey { get; set; }
        public string AkismetBlogUrl { get; set; }
        public string GitHubOwner { get; set; }
        public string GitHubRepository { get; set; }
        public string GitHubBranch { get; set; }
        public string GitHubCommentPath { get; set; }
        public string GitHubToken { get; set; }
    }
}
