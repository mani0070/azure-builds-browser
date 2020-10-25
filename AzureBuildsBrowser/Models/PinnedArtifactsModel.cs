using AzureBuildsBrowser.Clients;
using AzureBuildsBrowser.Clients.Models;

namespace AzureBuildsBrowser.Models
{
    public class PinnedArtifactsModel
    {
        public int BuildId { get; set; }
        public ArtifactDetails[] Artifacts { get; set; }
    }
}