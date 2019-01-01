using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StaticCommentsToGit.Entities;

namespace StaticCommentsToGit.Factories
{
    static class FormContentsFactory
    {
        public static async Task<FormContents> Create(HttpRequest req)
        {
            var collection = await req.ReadFormAsync();

            return new FormContents
            {
                Fields = new FormFields
                {
                    Body = collection["fields[body]"],
                    Email = collection["fields[email]"],
                    Name = collection["fields[name]"],
                    ReplyTo = collection["fields[replyTo]"]
                },
                Options = new FormOptions
                {
                    Recaptcha = new FormOptionsRecaptcha
                    {
                        Token = collection["options[reCaptcha][token]"]
                    },
                    Slug = collection["options[slug]"]
                }
            };
        }
    }
}
