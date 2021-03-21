using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.UnifiedRedisPlatform.FunctionalTests
{
    public class UnifiedRedisClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _tokenRelativeUri;
        private readonly string _configurationRelativeUri;

        public UnifiedRedisClient(TestContext testContext)
        {
            string endpoint = testContext.Properties["FunctionalTest:Endpoint"].ToString();
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(endpoint)
            };
            _tokenRelativeUri = testContext.Properties["FunctionalTest:TokenUri"].ToString();
            _configurationRelativeUri = testContext.Properties["FunctionalTest:ConfigUri"].ToString();
        }

        public async Task<ConfigurationResponse> GetConfiguration(string clusterName, string appName, string password, string region)
        {
            TokenResponse tokenResponse = await GetToken(clusterName, appName, password);
            string token = tokenResponse.Token;
            var request = new HttpRequestMessage(HttpMethod.Get, _configurationRelativeUri);
            request.Headers.Add("Authorization", $"Bearer {token}");
            if (!string.IsNullOrWhiteSpace(region))
                request.Headers.Add("x-location", region);
            
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string serializedConfigurationResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ConfigurationResponse>(serializedConfigurationResponse);
        }

        private async Task<TokenResponse> GetToken(string clusterName, string appName, string password)
        {
            var tokenRequest = new TokenRequest(clusterName, appName);
            var request = new HttpRequestMessage(HttpMethod.Post, _tokenRelativeUri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-URP-secret", password);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string serializedTokenResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenResponse>(serializedTokenResponse);
        }
    }
}
