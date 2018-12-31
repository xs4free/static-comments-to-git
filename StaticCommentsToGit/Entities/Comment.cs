using System;

namespace StaticCommentsToGit.Entities
{
    class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ReplyTo { get; set; } = Guid.Empty;
        public string Name { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
