using System;

namespace StaticCommentsToGit
{
    public class RecaptchaResponse
    {
        public bool success { get; set; }
        public decimal score { get; set; }
        public string action { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
    }
}
