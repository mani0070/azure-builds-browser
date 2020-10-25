using System.IO.Compression;

namespace AzureBuildsBrowser.Models
{
    public class ArtifactModel
    {
        public string Tag { get; set; }
        public string Repository { get; set; }
        public string Artifact { get; set; }
        public ZipArchive Zip { get; set; }
    }
}