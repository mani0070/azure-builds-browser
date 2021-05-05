namespace AzureBuildsBrowser.Clients.Models
{
    public class ArtifactDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ArtifactResource Resource { get; set; }
    }
}