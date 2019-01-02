using System;
using StaticCommentsToGit.Entities;

namespace StaticCommentsToGit.Factories
{
    static class SettingsFactory
    {
        public static Settings Create()
        {
            return new Settings()
            {
                ReCaptchaSecretKey = GetSetting(Setting.ReCaptchaSecretKey),
                ReCaptchaHostname = GetSetting(Setting.ReCaptchaHostname),
                ReCaptchaAction = GetSetting(Setting.ReCaptchaAction),
                ReCaptchaMinimumScore = GetSettingDecimal(Setting.ReCaptchaMinimumScore),
                AkismetApiKey = GetSetting(Setting.AkismetApiKey),
                AkismetBlogUrl = GetSetting(Setting.AkismetBlogUrl),
                GitHubOwner = GetSetting(Setting.GitHubOwner),
                GitHubRepository = GetSetting(Setting.GitHubRepository),
                GitHubBranch = GetSetting(Setting.GitHubBranch),
                GitHubCommentPath = GetSetting(Setting.GitHubCommentPath),
                GitHubToken = GetSetting(Setting.GitHubToken)
            };
        }

        private static string GetSetting(Setting setting) => Environment.GetEnvironmentVariable(setting.ToString(), EnvironmentVariableTarget.Process);

        private static decimal GetSettingDecimal(Setting setting)
        {
            string settingValue = GetSetting(setting);
            return Convert.ToDecimal(settingValue);
        }
    }
}
