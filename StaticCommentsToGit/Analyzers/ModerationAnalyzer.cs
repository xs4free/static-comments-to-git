using System;
using System.Text;
using Microsoft.Extensions.Logging;
using StaticCommentsToGit.Entities;

namespace StaticCommentsToGit.Analyzers
{
    class ModerationAnalyzer
    {
        private readonly Settings _settings;
        private readonly ILogger _log;

        public ModerationAnalyzer(Settings settings, ILogger log)
        {
            _settings = settings;
            _log = log;
        }

        public ModerationAnalysisReport NeedsModeration(Comment comment, RecaptchaResponse recaptchaResponse, AkismetResponse akismetResponse)
        {
            var reasonForModeration = new StringBuilder();

            CheckReCaptcha(recaptchaResponse, reasonForModeration);
            CheckAkismet(akismetResponse, reasonForModeration);

            //TODO: add new user check

            if (reasonForModeration.Length > 0)
            {
                _log.LogInformation("Comment needs moderation because: {0}", reasonForModeration.ToString());
            }
            else
            {
                _log.LogInformation("Comment does not need moderation");
            }

            return new ModerationAnalysisReport
            {
                NeedsModeration = reasonForModeration.Length > 0,
                ReasonForModeration = reasonForModeration.ToString()
            };
        }

        private void CheckAkismet(AkismetResponse akismetResponse, StringBuilder reasonForModeration)
        {
            if (akismetResponse.IsSpam)
            {
                reasonForModeration.AppendLine("Akismet says it's spam");
            }

            if (string.Compare(akismetResponse.ProTip, "discard", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                reasonForModeration.AppendLine("Akismet pro-tip is 'discard'");
            }
        }

        private void CheckReCaptcha(RecaptchaResponse recaptchaResponse, StringBuilder reasonForModeration)
        {
            if (!recaptchaResponse.success)
            {
                reasonForModeration.AppendLine("reCAPTCHA success is false");
            }

            if (recaptchaResponse.score < _settings.ReCaptchaMinimumScore)
            {
                reasonForModeration.AppendLine(
                    $"reCAPTCHA score < minimum required score ({recaptchaResponse.score} < {_settings.ReCaptchaMinimumScore})");
            }

            if (string.Compare(recaptchaResponse.hostname, _settings.ReCaptchaHostname,
                    StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                reasonForModeration.AppendLine(
                    $"reCAPTCHA hostname != expected hostname ('{recaptchaResponse.hostname}' != '{_settings.ReCaptchaHostname}')");
            }

            if (string.Compare(recaptchaResponse.hostname, _settings.ReCaptchaHostname,
                    StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                reasonForModeration.AppendLine(
                    $"reCAPTCHA action != expected action ('{recaptchaResponse.action}' != '{_settings.ReCaptchaAction}')");
            }
        }
    }
}
