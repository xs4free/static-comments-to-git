using System;
using System.Text;
using StaticCommentsToGit.Entities;

namespace StaticCommentsToGit.Mappers
{
    static class CommentSerializer
    {
        public static string SerializeToYaml(Comment comment)
        {
            StringBuilder yaml = new StringBuilder();

            yaml.AppendFormat("id: {0:D}", comment.Id);
            yaml.AppendLine();

            if (comment.ReplyTo == Guid.Empty)
            {
                yaml.AppendLine("replyTo: 0");
            }
            else
            {
                yaml.AppendFormat("replyTo: {0:D}", comment.ReplyTo);
                yaml.AppendLine();
            }

            yaml.Append("name: ");
            yaml.AppendLine(comment.Name);

            yaml.Append("email: ");
            yaml.AppendLine(comment.Email);

            yaml.Append("body: ");
            yaml.AppendLine(comment.Body);

            yaml.AppendFormat("date: {0:O}", comment.Date);
            yaml.AppendLine();

            yaml.Append("slug: ");
            yaml.AppendLine(comment.Slug);

            return yaml.ToString();
        }
    }
}
