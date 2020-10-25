using AzureBuildsBrowser.Clients.Models;

namespace AzureBuildsBrowser.Models
{
    public class BuildsModel
    {
        public BuildDetails[] Builds { get; set; }
        public string Tag { get; set; }
    }
}