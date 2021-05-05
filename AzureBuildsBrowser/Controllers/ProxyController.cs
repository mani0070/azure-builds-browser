using System;
using System.Linq;
using System.Threading.Tasks;
using AzureBuildsBrowser.Clients;
using AzureBuildsBrowser.Clients.Models;
using AzureBuildsBrowser.Models;
using AzureBuildsBrowser.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace AzureBuildsBrowser.Controllers
{
    public class ProxyController : Controller
    {
        private readonly IDevopsClient _client;
        private readonly IContentTypeProvider _contentTypeProvider;

        public ProxyController(IDevopsClient client, IContentTypeProvider contentTypeProvider)
        {
            _client = client;
            _contentTypeProvider = contentTypeProvider;
        }

        [Route("azdevops")]
        public async Task<IActionResult> Root()
        {
            var tagsTask = _client.GetBuildTags();
            var buildTask = _client.FindLastBuilds(20);
            var tags = await tagsTask;
            var lastBuilds = await buildTask;

            return View(new RootModel { Tags = tags, LastBuilds = lastBuilds });
        }

        [Route("azdevops/t/{tag}")]
        public async Task<IActionResult> Builds(string tag)
        {
            var builds = await _client.FindBuilds(tag);
            return View(new BuildsModel { Builds = builds, Tag = tag });
        }

        [Route("azdevops/b/{buildId}")]
        public async Task<IActionResult> PinnedArtifacts(int buildId)
        {
            var artifacts = await _client.FindArtifacts(buildId);

            return View(new PinnedArtifactsModel
            {
                BuildId = buildId,
                Artifacts = artifacts
            });
        }

        [Route("azdevops/t/{tag}/r/{repository}")]
        public async Task<IActionResult> Artifacts(string tag, string repository)
        {
            var build = await GetLastBuild(tag, repository);
            if (build == null)
                return NotFound();

            var artifacts = await _client.FindArtifacts(build.Id);

            return View(new ArtifactsModel
            {
                Tag = tag,
                Repository = repository,
                Artifacts = artifacts
            });
        }

        [Route("azdevops/t/{tag}/r/{repository}/a/{artifactName}")]
        public async Task<IActionResult> Artifact(string tag, string repository, string artifactName)
        {
            var build = await GetLastBuild(tag, repository);
            if (build == null)
                return NotFound();

            var artifact = await _client.GetArtifact(build.Id, artifactName);
            var zip = await _client.DownloadZipFile(artifact.Resource.DownloadUrl);

            return View(new ArtifactModel
            {
                Tag = tag,
                Repository = repository,
                Artifact = artifactName,
                Zip = zip
            });
        }

        [Route("azdevops/b/{buildId}/a/{artifactName}")]
        public async Task<IActionResult> PinnedArtifact(int buildId, string artifactName)
        {
            var artifact = await _client.GetArtifact(buildId, artifactName);
            var zip = await _client.DownloadZipFile(artifact.Resource.DownloadUrl);

            return View(new PinnedArtifactModel
            {
                BuildId = buildId,
                Artifact = artifactName,
                Zip = zip
            });
        }

        [Route("azdevops/t/{tag}/r/{repository}/a/{artifactName}/f/{**filePath}")]
        public async Task<IActionResult> File(string tag, string repository, string artifactName, string filePath)
        {
            var build = await GetLastBuild(tag, repository);
            if (build == null)
                return NotFound();

            return await ReturnZipFile(build.Id, artifactName, filePath);
        }

        [Route("azdevops/b/{buildId}/a/{artifactName}/f/{**filePath}")]
        public async Task<IActionResult> PinnedFile(int buildId, string artifactName, string filePath)
        {
            return await ReturnZipFile(buildId, artifactName, filePath);
        }

        private async Task<IActionResult> ReturnZipFile(int buildId, string artifactName, string filePath)
        {
            var artifact = await _client.GetArtifact(buildId, artifactName);
            if (artifact == null)
                return NotFound();

            var zip = await _client.DownloadZipFile(artifact.Resource.DownloadUrl);
            var pathRegex = PathGlob.Create(filePath);
            var entry = zip.Entries
                .OrderByDescending(e => e.LastWriteTime)
                .FirstOrDefault(x => pathRegex.IsMatch(x.FullName));
            if (entry == null)
                return NotFound();

            var contentType = _contentTypeProvider.TryGetContentType(filePath, out var c) ? c : "application/octet-stream";
            return File(entry.Open(), contentType);
        }

        private async Task<BuildDetails> GetLastBuild(string tag, string repository)
        {
            return (await _client.FindBuilds(tag)).SingleOrDefault(b => string.Equals(b.Repository.Name, repository, StringComparison.OrdinalIgnoreCase));
        }
    }
}
