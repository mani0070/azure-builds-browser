using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureBuildsBrowser.Clients.Models;
using AzureBuildsBrowser.Utils;
using Microsoft.Extensions.Options;

namespace AzureBuildsBrowser.Clients
{
    public class DevopsClient : IDevopsClient
    {
        private readonly HttpClient _client;

        public DevopsClient(HttpClient client, IOptions<DevopClientOptions> options)
        {
            _client = client;
            _client.BaseAddress = new Uri($"{options.Value.ProjectUri}/_apis/");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Encode(options));
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static string Encode(IOptions<DevopClientOptions> options) => Convert.ToBase64String(Encoding.ASCII.GetBytes($":{options.Value.PersonalAccessToken}"));

        public async Task<string[]> GetBuildTags()
        {
            var collection = await _client.GetFromJsonAsync<Collection<string>>("build/tags?api-version=6.0");
            return collection.Value;
        }

        public async Task<BuildDetails[]> FindBuilds(string tag)
        {
            var collection = await _client.GetFromJsonAsync<Collection<BuildDetails>>($"build/builds?api-version=6.0&tagFilters={tag}");
            return collection.Value;
        }

        public async Task<BuildDetails[]> FindLastBuilds(int topCount)
        {
            var collection = await _client.GetFromJsonAsync<Collection<BuildDetails>>($"build/builds?api-version=6.0&queryOrder=finishTimeDescending&statusFilter=completed&$top={topCount}");
            return collection.Value;
        }

        public async Task<ArtifactDetails[]> FindArtifacts(int buildId)
        {
            var collection = await _client.GetFromJsonAsync<Collection<ArtifactDetails>>($"build/builds/{buildId}/artifacts?api-version=6.0");
            return collection.Value;
        }

        public async Task<ArtifactDetails> GetArtifact(int buildId, string artifactName)
        {
            if (PathGlob.HasWildcards(artifactName))
                return await SearchForLatestArtifact(buildId, artifactName);
            var artifact = await _client.GetFromJsonAsync<ArtifactDetails>($"build/builds/{buildId}/artifacts?artifactName={artifactName}&api-version=6.0");
            return artifact;
        }

        private async Task<ArtifactDetails> SearchForLatestArtifact(int buildId, string artifactName)
        {
            var artifactRegex = PathGlob.Create(artifactName);
            var artifacts = await FindArtifacts(buildId);
            var artifact = artifacts
                .Where(a => artifactRegex.IsMatch(a.Name))
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();
            return artifact;
        }

        public async Task<ZipArchive> DownloadZipFile(string resourceUri)
        {
            var bytes = await _client.GetByteArrayAsync(resourceUri);
            return new ZipArchive(new MemoryStream(bytes));
        }
    }
}