using System.Security.Cryptography;
using System.Text;
using StaticCommentsToGit.Entities;

namespace StaticCommentsToGit.Factories
{
    static class CommentFactory
    {
        public static Comment Create(FormContents form)
        {
            var comment = new Comment
            {
                Name = form.Fields.Name,
                Email = HashEmailForGravatar(form.Fields.Email),
                Body = form.Fields.Body,
                ReplyTo = form.Fields.ReplyTo,
                Slug = form.Options.Slug
            };

            return comment;
        }

        private static string HashEmailForGravatar(string input)
        {
            input = input.Trim().ToLowerInvariant();

            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                foreach (var hashByte in hashBytes)
                {
                    sb.Append(hashByte.ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
