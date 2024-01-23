using System.Text.Json;
using MicrosoftGraphTestApp.Data;
using MicrosoftGraphTestApp.GraphData;

namespace MicrosoftGraphTestApp
{
    public class GraphHelper
    {
        private readonly HttpClient _httpClient;
        
        // Settings object
        private readonly Settings _settings;

        private static string? _accessToken;
        
        public GraphHelper(Settings settings, HttpClient? httpClient = null)
        {
            _settings = settings;
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<GraphResponse<List<Group>>>GetGroupsAsync()
        {
            return await SendGraphRequestAsync<List<Group>>("groups", HttpMethod.Get);
        }

        private async Task<string?> GetAccessTokenAsync()
        {
            var parameters = new Dictionary<string, string>
            {
                {"client_id", _settings.ClientId},
                {"grant_type", _settings.GrantType},
                {"scope", _settings.Scope},
                {"client_secret", _settings.ClientSecret}
            };

            var token = await SendIdentityRequestAsync<TokenResponse>(
                "token", HttpMethod.Post, parameters);

            return token?.AccessToken;
        }
        
        private async Task<string> SendRequestAsync(string uri, HttpMethod method,
            string? accessToken, Dictionary<string, string>? parameters = null)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                
            var request = new HttpRequestMessage(method, uri);

            if (parameters != null)
                request.Content = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException("Invalid credentials or input data");
            
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<GraphResponse<T>> SendGraphRequestAsync<T>(string action, HttpMethod method)
        {
            _accessToken ??= await GetAccessTokenAsync();
            
            string response = await SendRequestAsync(
                $"{_settings.GraphApiUrl}/{action}", method, _accessToken);

            var graphCallResponse = JsonSerializer.Deserialize<GraphCallResponse<T>>(response);
            
            return new GraphResponse<T>
            {
                Response = response,
                Value = graphCallResponse == null ? default : graphCallResponse.Value
            };
        }

        private async Task<T?> SendIdentityRequestAsync<T>(string action, HttpMethod method,
            Dictionary<string, string>? parameters = null)
        {
            string response = await SendRequestAsync(
                $"{_settings.IdentityApiUrl}/{action}", method, null, parameters);
            return JsonSerializer.Deserialize<T>(response);
        }
    }
}