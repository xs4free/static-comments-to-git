using System;
using System.IO;
using System.Linq;
using System.Text;
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
        private readonly GitHubClient _github;
        private readonly string _knownCommentersPath;

        public GitHubService(string owner, string repo, string branch, string commentDataPath, string token)
        {
            _owner = owner;
            _repo = repo;
            _branch = branch;
            _commentDataPath = commentDataPath;

            _github = new GitHubClient(new ProductHeaderValue("StaticCommentsToGit"));
            _github.Credentials = new Credentials(token);

            _knownCommentersPath = Path.Combine(_commentDataPath, "known-commenters.csv");
        }

        public async Task AddComment(Comment comment, ModerationAnalysisReport report, KnownCommenterResponse knownCommenterResponse)
        {
            string yaml = CommentSerializer.SerializeToYaml(comment);

            var message = $"Add comment to '{comment.Slug}' by '{comment.Name}'";
            var path = Path.Combine(_commentDataPath, comment.Slug, $"comment-{comment.Date.Ticks}.yml");
            var branch = _branch;

            if (report.NeedsModeration)
            {
                branch = $"sc2g-{comment.Slug}-{comment.Date.Ticks}";
                await CreateNewBranch(branch);
            }

            var createFileRequest = new CreateFileRequest(message, yaml, branch);
            await _github.Repository.Content.CreateFile(_owner, _repo, path, createFileRequest);

            if (!knownCommenterResponse.IsKnownCommenter)
            {
                await CreateOrUpdateKnownCommentersFile(knownCommenterResponse, branch);
            }

            if (report.NeedsModeration)
            {
                var newPullRequest = new NewPullRequest(message, branch, _branch);
                newPullRequest.Body = report.ReasonForModeration;
                await _github.Repository.PullRequest.Create(_owner, _repo, newPullRequest);
            }
        }

        private async Task CreateOrUpdateKnownCommentersFile(KnownCommenterResponse knownCommenterResponse, string branch)
        {
            var message = $"Add known commenter '{knownCommenterResponse.Username}'";
            var content = AddKnownCommenterToCsvContent(knownCommenterResponse);

            if (knownCommenterResponse.FileExists)
            {
                var updateFileRequest = new UpdateFileRequest(message, content, knownCommenterResponse.Sha, branch);
                await _github.Repository.Content.UpdateFile(_owner, _repo, _knownCommentersPath, updateFileRequest);
            }
            else
            {
                var createFileRequest = new CreateFileRequest(message, content, branch);
                await _github.Repository.Content.CreateFile(_owner, _repo, _knownCommentersPath, createFileRequest);
            }
        }

        private static string AddKnownCommenterToCsvContent(KnownCommenterResponse knownCommenterResponse)
        {
            var allCommenters = new StringBuilder(knownCommenterResponse.KnownCommenters);

            string row = CreateKnownCommenterRow(knownCommenterResponse.Username, knownCommenterResponse.Email);
            allCommenters.AppendLine(row);

            return allCommenters.ToString();
        }

        public async Task<KnownCommenterResponse> IsKnownCommenter(string username, string email)
        {
            var response = new KnownCommenterResponse
            {
                Username = username,
                Email = email,
            };

            try
            {
                var knownCommentersContents =
                    await _github.Repository.Content.GetAllContents(_owner, _repo, _knownCommentersPath);

                var knownCommentersContent = knownCommentersContents.FirstOrDefault();
                response.FileExists = knownCommentersContent != null;
                response.KnownCommenters = knownCommentersContent?.Content;
                response.Sha = knownCommentersContent?.Sha;

                var row = CreateKnownCommenterRow(username, email);
                response.IsKnownCommenter = response.KnownCommenters?.Contains(row, StringComparison.InvariantCultureIgnoreCase) ?? false;
            }
            catch (Octokit.NotFoundException)
            {
            }

            return response;
        }

        private static string CreateKnownCommenterRow(string username, string email)
        {
            return $"{email},{username.Trim().ToLowerInvariant()}";
        }

        private async Task CreateNewBranch(string newBranchName)
        {
            var headRef = $"heads/{_branch}";
            var targetBranchReference = await _github.Git.Reference.Get(_owner, _repo, headRef);

            var newBranchRef = $"refs/heads/{newBranchName}";
            var newReference = new NewReference(newBranchRef, targetBranchReference.Object.Sha);
            await _github.Git.Reference.Create(_owner, _repo, newReference);
        }
    }
}
