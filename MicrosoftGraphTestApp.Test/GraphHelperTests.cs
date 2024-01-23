using System.Net;
using System.Text.Json;
using MicrosoftGraphTestApp.Data;
using MicrosoftGraphTestApp.GraphData;
using RichardSzalay.MockHttp;

namespace MicrosoftGraphTestApp.Test
{
    public class GraphHelperTests
    {
        private const string TestAccessToken = "AccessToken";
        
        private const string IdentityApiUrl = "https://login.microsoftonline.com/TenantId/oauth2/v2.0";
        private const string GraphApiUrl = "https://graph.microsoft.com/v1.0";
        
        private const string TestClientId = "ClientId";
        private const string TestClientSecret = "ClientSecret";
        private const string TestScope = "https://graph.microsoft.com/.default";
        private const string TestGrantType = "client_credentials";
        
        private const string TestGroup = "Group";
        
        [Fact]
        public async void GetGroups_Success()
        {
            GraphHelper graphHelper = GetGraphHelper(out string jsonResponse, false);

            try
            {
                Assert.NotNull(graphHelper);
                var groupsResponse = await graphHelper.GetGroupsAsync();
                if (groupsResponse == null || groupsResponse.Value == null)
                    throw new NullReferenceException("Error getting groups response.");
                
                Assert.Equal(jsonResponse, groupsResponse.Response);
                Assert.Single(groupsResponse.Value);
                Assert.Equal(TestGroup, groupsResponse.Value.Single().DisplayName);
            }
            catch (Exception e)
            { 
                Assert.Fail("Error downloading groups: " + e.Message);
            }
        }
        
        [Fact]
        public async void GetGroups_InvalidClientSecret()
        {
            GraphHelper graphHelper = GetGraphHelper(out string jsonResponse, true);
            
            Assert.NotNull(graphHelper);
            await Assert.ThrowsAsync<ArgumentException>(() => graphHelper.GetGroupsAsync());
        }

        private GraphHelper GetGraphHelper(out string jsonResponse, bool invalidClientSecret)
        {
            var mockHttp = new MockHttpMessageHandler();
            
            var parameters = new Dictionary<string, string>
            {
                {"client_id", TestClientId},
                {"grant_type", TestGrantType},
                {"scope", TestScope},
                {"client_secret", TestClientSecret}
            };

            if (invalidClientSecret)
            {
                mockHttp.Expect($"{IdentityApiUrl}/token")
                    .WithExactFormData(parameters)
                    .Respond(HttpStatusCode.Unauthorized);
            }
            else
            {
                mockHttp.Expect($"{IdentityApiUrl}/token")
                    .WithExactFormData(parameters)
                    .Respond("application/json", JsonSerializer.Serialize(
                        new TokenResponse {AccessToken = TestAccessToken}));
            }

            jsonResponse = JsonSerializer.Serialize(
                new GraphCallResponse<List<Group>>
                    {Value = new List<Group> {new() {DisplayName = TestGroup}}});
            
            mockHttp.When($"{GraphApiUrl}/groups")
                .WithHeaders(new Dictionary<string,string> {{ "Authorization", $"Bearer {TestAccessToken}" }})
                .Respond("application/json", jsonResponse);
            
            var httpClient = mockHttp.ToHttpClient();
            
            GraphHelper? graphHelper = null;
            try
            {
                graphHelper = new GraphHelper(Settings.LoadSettings(), httpClient);
            }
            catch (Exception e)
            {
                Assert.Fail("Error initializing Graph: " + e.Message);
            }

            return graphHelper;
        }
    }
}