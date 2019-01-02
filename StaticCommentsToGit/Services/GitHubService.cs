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

        public async Task AddComment(Comment comment, ModerationAnalysisReport report)
        {
            string yaml = CommentSerializer.SerializeToYaml(comment);

            var github = new GitHubClient(new ProductHeaderValue("StaticCommentsToGit"));
            github.Credentials = new Credentials(_token);

            var message = $"Add comment by {comment.Name}";
            var path = Path.Combine(_commentDataPath, comment.Slug, $"comment-{comment.Date.Ticks}.yml");
            var branch = _branch;

            if (report.NeedsModeration)
            {
                branch = $"sc2g-{comment.Slug}-{comment.Date.Ticks}";
                await CreateNewBranch(github, branch);
            }

            var createFileRequest = new CreateFileRequest(message, yaml, branch);
            await github.Repository.Content.CreateFile(_owner, _repo, path, createFileRequest);

            if (report.NeedsModeration)
            {
                var newPullRequest = new NewPullRequest(report.ReasonForModeration, branch, _branch);
                await github.Repository.PullRequest.Create(_owner, _repo, newPullRequest);
            }
        }

        private async Task CreateNewBranch(GitHubClient github, string newBranchName)
        {
            var headRef = $"heads/{_branch}";
            var targetBranchReference = await github.Git.Reference.Get(_owner, _repo, headRef);

            var newBranchRef = $"refs/heads/{newBranchName}";
            var newReference = new NewReference(newBranchRef, targetBranchReference.Object.Sha);
            await github.Git.Reference.Create(_owner, _repo, newReference);
        }
    }
}
