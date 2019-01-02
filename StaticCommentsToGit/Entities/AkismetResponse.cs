namespace StaticCommentsToGit.Entities
{
    class AkismetResponse
    {
        public bool IsSpam { get; set; }
        public string Text { get; set; }
        public string ProTip { get; set; }
        public string DebugHelp { get; set; }

        public override string ToString()
        {
            return $"IsSpam: {IsSpam}, Text: '{Text}', ProTip: '{ProTip}', DebugHelp: '{DebugHelp}'";
        }
    }
}
