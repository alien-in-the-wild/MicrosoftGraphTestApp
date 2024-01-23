using System.Text.Json.Serialization;

namespace MicrosoftGraphTestApp.Data;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
}