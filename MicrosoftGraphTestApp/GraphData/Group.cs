using System.Text.Json.Serialization;

namespace MicrosoftGraphTestApp.GraphData
{
    /// <summary>
    /// Group object from Graph API
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Group display name
        /// </summary>
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }
    }
}