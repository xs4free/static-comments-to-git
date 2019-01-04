using System;
using Microsoft.Extensions.Logging;
using StaticCommentsToGit.Entities;

namespace StaticCommentsToGit.Factories
{
    static class SettingsFactory
    {
        private const string SitesSeparator = ",";

        public static Settings Create(string commentOrigin, ILogger log)
        {
            string siteName = GetSiteNameForOrigin(commentOrigin);

            log.LogDebug("Using settings for site: '{0}'", siteName ?? "<default>");

            return CreateForSite(siteName);
        }

        private static string GetSiteNameForOrigin(string commentOrigin)
        {
            var siteOrigins = GetSetting(Setting.SiteOrigins.ToString())?.Split(SitesSeparator, StringSplitOptions.RemoveEmptyEntries);

            string siteName = null;

            for (int index = 0; index < siteOrigins?.Length; index++)
            {
                var siteOrigin = siteOrigins[index];
                if (!commentOrigin.Contains(siteOrigin, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var siteNames = GetSetting(Setting.SiteNames.ToString())?.Split(SitesSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (siteNames?.Length > index)
                {
                    siteName = siteNames[index];
                    break;
                }
            }

            return siteName;
        }

        private static Settings CreateForSite(string siteName)
        {
            return new Settings
            {
                ReCaptchaSecretKey = GetSetting(siteName, Setting.ReCaptchaSecretKey),
                ReCaptchaHostname = GetSetting(siteName, Setting.ReCaptchaHostname),
                ReCaptchaAction = GetSetting(siteName, Setting.ReCaptchaAction),
                ReCaptchaMinimumScore = GetSettingDecimal(siteName, Setting.ReCaptchaMinimumScore),
                AkismetApiKey = GetSetting(siteName, Setting.AkismetApiKey),
                AkismetBlogUrl = GetSetting(siteName, Setting.AkismetBlogUrl),
                GitHubOwner = GetSetting(siteName, Setting.GitHubOwner),
                GitHubRepository = GetSetting(siteName, Setting.GitHubRepository),
                GitHubBranch = GetSetting(siteName, Setting.GitHubBranch),
                GitHubCommentPath = GetSetting(siteName, Setting.GitHubCommentPath),
                GitHubToken = GetSetting(siteName, Setting.GitHubToken)
            };
        }

        private static string GetSetting(string siteName, Setting setting)
        {
            string value;

            if (siteName == null)
            {
                value = GetSetting(setting.ToString());
            }
            else
            {
                value = GetSetting($"{siteName}-{setting}") ??
                        GetSetting(setting.ToString());
            }

            return value;
        }

        private static string GetSetting(string key) =>
            Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);

        private static decimal GetSettingDecimal(string siteName, Setting setting)
        {
            string settingValue = GetSetting(siteName, setting);
            return Convert.ToDecimal(settingValue);
        }
    }
}
