using System.Text.Json.Serialization;

namespace MicrosoftGraphTestApp.Data
{
    /// <summary>
    /// Response from Identity API token request
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// Token string
        /// </summary>
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }
}