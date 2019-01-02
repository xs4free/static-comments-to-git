using System;

namespace StaticCommentsToGit.Entities
{
    class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ReplyTo { get; set; } = string.Empty; //string instead of Guid for backwards-compatibility
        public string Name { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Slug { get; set; }
    }
}
