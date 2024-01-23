using System.Text.Json;
using MicrosoftGraphTestApp.Data;
using MicrosoftGraphTestApp.GraphData;

namespace MicrosoftGraphTestApp
{
    /// <summary>
    /// Helper to call MS Graph API
    /// </summary>
    public class GraphHelper
    {
        /// <summary>
        /// HttpClient to call APIs
        /// </summary>
        private readonly HttpClient _httpClient;
        
        /// <summary>
        /// Settings object
        /// </summary>
        private readonly Settings _settings;

        /// <summary>
        /// Token to access MS Graph API
        /// </summary>
        private static string? _accessToken;
        
        /// <summary>
        /// Creates a new instance of GraphHelper
        /// </summary>
        /// <param name="settings">Settings object</param>
        /// <param name="httpClient">Optional HttpClient to call APIs</param>
        public GraphHelper(Settings settings, HttpClient? httpClient = null)
        {
            _settings = settings;
            _httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Fetches Groups from MS Graph API
        /// </summary>
        /// <returns>Response with a list of Groups</returns>
        public async Task<GraphResponse<List<Group>>>GetGroupsAsync()
        {
            return await SendGraphRequestAsync<List<Group>>("groups", HttpMethod.Get);
        }

        /// <summary>
        /// Gets Token from Identity Api
        /// </summary>
        /// <returns>Token from Identity Api</returns>
        private async Task<string?> GetAccessTokenAsync()
        {
            // Get parameters from settings
            var parameters = new Dictionary<string, string>
            {
                {"client_id", _settings.ClientId},
                {"grant_type", _settings.GrantType},
                {"scope", _settings.Scope},
                {"client_secret", _settings.ClientSecret}
            };

            // Get Token
            var token = await SendIdentityRequestAsync<TokenResponse>(
                "token", HttpMethod.Post, parameters);

            return token?.AccessToken;
        }
        
        /// <summary>
        /// Sends http request to API
        /// </summary>
        /// <param name="uri">Url to request</param>
        /// <param name="method">Method to request</param>
        /// <param name="accessToken">Access token for Authorization</param>
        /// <param name="parameters">Dictionary of parameters Form Url Encoded Content </param>
        /// <returns>JSON string of the response</returns>
        private async Task<string> SendRequestAsync(string uri, HttpMethod method,
            string? accessToken, Dictionary<string, string>? parameters = null)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            
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

        /// <summary>
        /// Sends http request to Graph API
        /// </summary>
        /// <param name="action">Action to request</param>
        /// <param name="method">Method to request</param>
        /// <typeparam name="T">Object type to cast the response to</typeparam>
        /// <returns>Casted response object</returns>
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

        /// <summary>
        /// Sends http request to Identity API
        /// </summary>
        /// <param name="action">Action to request</param>
        /// <param name="method">Method to request</param>
        /// <param name="parameters">Dictionary of parameters Form Url Encoded Content </param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private async Task<T?> SendIdentityRequestAsync<T>(string action, HttpMethod method,
            Dictionary<string, string>? parameters = null)
        {
            string response = await SendRequestAsync(
                $"{_settings.IdentityApiUrl}/{action}", method, null, parameters);
            return JsonSerializer.Deserialize<T>(response);
        }
    }
}