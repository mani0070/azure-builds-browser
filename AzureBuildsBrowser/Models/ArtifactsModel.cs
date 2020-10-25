using AzureBuildsBrowser.Clients;
using AzureBuildsBrowser.Clients.Models;

namespace AzureBuildsBrowser.Models
{
    public class ArtifactsModel
    {
        public string Tag { get; set; }
        public string Repository { get; set; }
        public ArtifactDetails[] Artifacts { get; set; }
    }
}