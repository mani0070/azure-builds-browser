using System.IO.Compression;
using System.Threading.Tasks;
using AzureBuildsBrowser.Clients.Models;

namespace AzureBuildsBrowser.Clients
{
    public interface IDevopsClient
    {
        Task<string[]> GetBuildTags();
        Task<BuildDetails[]> FindBuilds(string tag);
        Task<BuildDetails[]> FindLastBuilds(int topCount);
        Task<ArtifactDetails[]> FindArtifacts(int buildId);
        Task<ArtifactDetails> GetArtifact(int buildId, string artifactName);
        Task<ZipArchive> DownloadZipFile(string resourceUri);
    }
}