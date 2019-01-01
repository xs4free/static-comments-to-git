using System.IO;
using System.Threading.Tasks;
using Octokit;
using StaticCommentsToGit.Entities;
using StaticCommentsToGit.Mappers;

namespace StaticCommentsToGit.Services
{
    class GitHubService
    {
        private readonly string _owner;
        private readonly string _repo;
        private readonly string _branch;
        private readonly string _commentDataPath;
        private readonly string _token;

        public GitHubService(string owner, string repo, string branch, string commentDataPath, string token)
        {
            _owner = owner;
            _repo = repo;
            _branch = branch;
            _commentDataPath = commentDataPath;
            _token = token;
        }

        public async Task AddComment(Comment comment)
        {
            string yaml = CommentSerializer.SerializeToYaml(comment);

            var github = new GitHubClient(new ProductHeaderValue("StaticCommentsToGit"));
            github.Credentials = new Credentials(_token);

            // github variables
            var message = $"Add comment by {comment.Name}";
            var createFileRequest = new CreateFileRequest(message, yaml, _branch);

            var path = Path.Combine(_commentDataPath, comment.Slug, $"comment-{comment.Date.Ticks}.yml");

            await github.Repository.Content.CreateFile(_owner, _repo, path, createFileRequest);
        }
    }
}
