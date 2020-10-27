using System;
using System.Text.Json.Serialization;

namespace AzureBuildsBrowser.Clients.Models
{
    public class BuildDetails
    {
        public int Id { get; set; }
        public BuildRepository Repository { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset FinishTime { get; set; }
        public string Result { get; set; }
        public string SourceBranch { get; set; }
        [JsonPropertyName("_links")]
        public Links Links { get; set; }
    }
}