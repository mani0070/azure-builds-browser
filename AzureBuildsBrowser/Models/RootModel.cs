using AzureBuildsBrowser.Clients.Models;

namespace AzureBuildsBrowser.Models
{
    public class RootModel
    {
        public string[] Tags { get; set; }
        public BuildDetails[] LastBuilds { get; set; }
    }
}