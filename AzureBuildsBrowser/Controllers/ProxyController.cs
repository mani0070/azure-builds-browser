using System;
using System.Linq;
using System.Threading.Tasks;
using AzureBuildsBrowser.Clients;
using AzureBuildsBrowser.Clients.Models;
using AzureBuildsBrowser.Models;
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

        [Route("/")]
        public async Task<IActionResult> Tags()
        {
            var tags = await _client.GetBuildTags();
            return View(new BuildModel { Tags = tags });
        }

        [Route("t/{tag}")]
        public async Task<IActionResult> Builds(string tag)
        {
            var builds = await _client.FindBuilds(tag);
            return View(new BuildsModel { Builds = builds, Tag = tag });
        }

        [Route("b/{buildId}")]
        public async Task<IActionResult> PinnedArtifacts(int buildId)
        {
            var artifacts = await _client.FindArtifacts(buildId);

            return View(new PinnedArtifactsModel
            {
                BuildId = buildId,
                Artifacts = artifacts
            });
        }

        [Route("t/{tag}/r/{repository}")]
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

        [Route("t/{tag}/r/{repository}/a/{artifactName}")]
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

        [Route("b/{buildId}/a/{artifactName}")]
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

        [Route("t/{tag}/r/{repository}/a/{artifactName}/f/{fileName}")]
        public async Task<IActionResult> File(string tag, string repository, string artifactName, string fileName)
        {
            var build = await GetLastBuild(tag, repository);
            if (build == null)
                return NotFound();

            return await ReturnZipFile(build.Id, artifactName, fileName);
        }

        [Route("b/{buildId}/a/{artifactName}/f/{fileName}")]
        public async Task<IActionResult> PinnedFile(int buildId, string artifactName, string fileName)
        {
            return await ReturnZipFile(buildId, artifactName, fileName);
        }

        private async Task<IActionResult> ReturnZipFile(int buildId, string artifactName, string fileName)
        {
            fileName = Uri.UnescapeDataString(fileName);

            var artifact = await _client.GetArtifact(buildId, artifactName);
            if (artifact == null)
                return NotFound();

            var zip = await _client.DownloadZipFile(artifact.Resource.DownloadUrl);
            var entry = zip.Entries.FirstOrDefault(x =>
                string.Equals(x.FullName, fileName, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
                return NotFound();

            var contentType = _contentTypeProvider.TryGetContentType(fileName, out var c) ? c : "application/octet-stream";
            return File(entry.Open(), contentType);
        }

        private async Task<BuildDetails> GetLastBuild(string tag, string repository)
        {
            return (await _client.FindBuilds(tag)).SingleOrDefault(b => string.Equals(b.Repository.Name, repository, StringComparison.OrdinalIgnoreCase));
        }
    }
}
