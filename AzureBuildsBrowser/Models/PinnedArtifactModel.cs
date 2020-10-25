using System.IO.Compression;

namespace AzureBuildsBrowser.Models
{
    public class PinnedArtifactModel
    {
        public int BuildId { get; set; }
        public string Artifact { get; set; }
        public ZipArchive Zip { get; set; }
    }
}