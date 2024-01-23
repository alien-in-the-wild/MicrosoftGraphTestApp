using System.Text.Json.Serialization;

namespace MicrosoftGraphTestApp.GraphData
{
    public class Group
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }
    }
}