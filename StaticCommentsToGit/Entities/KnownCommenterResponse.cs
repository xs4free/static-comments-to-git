namespace StaticCommentsToGit.Entities
{
    class KnownCommenterResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsKnownCommenter { get; set; }

        public bool FileExists { get; set; }
        public string Sha { get; set; }
        public string KnownCommenters { get; set; }
    }
}
