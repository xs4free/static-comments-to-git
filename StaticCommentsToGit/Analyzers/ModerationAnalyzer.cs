using System;
using StaticCommentsToGit.Entities;

namespace StaticCommentsToGit.Analyzers
{
    class ModerationAnalyzer
    {
        private readonly Settings _settings;

        public ModerationAnalyzer(Settings settings)
        {
            _settings = settings;
        }

        public bool NeedsModeration(Comment comment, RecaptchaResponse recaptchaResponse)
        {
            bool validUser =
                recaptchaResponse.success
                && recaptchaResponse.score >= _settings.ReCaptchaMinimumScore
                && string.Compare(recaptchaResponse.hostname, _settings.ReCaptchaHostname,
                    StringComparison.InvariantCultureIgnoreCase) == 0
                && string.Compare(recaptchaResponse.action, _settings.ReCaptchaAction,
                    StringComparison.InvariantCultureIgnoreCase) == 0;

            //TODO: add Akismet check

            //TODO: add new user check

            return !validUser;
        }
    }
}
