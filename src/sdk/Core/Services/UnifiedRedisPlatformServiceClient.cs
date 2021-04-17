using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UnifiedRedisPlatform.Core.Exceptions;
using Microsoft.UnifiedRedisPlatform.Core.Services.Models;
using Microsoft.UnifiedRedisPlatform.Core.Services.Interfaces;

namespace Microsoft.UnifiedRedisPlatform.Core.Services
{
    internal class UnifiedRedisPlatformServiceClient : IUnifiedRedisPlatformServiceClient
    {
        private readonly string _serviceEndpoint;
        private readonly string _clusterName;
        private readonly string _appName;
        private readonly string _appSecret;
        private readonly string _preferredLocation;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICache _internalCache;

        public UnifiedRedisPlatformServiceClient(string serviceEndpoint, string clusterName, string appName, string appSecret, string preferredLocation, IHttpClientFactory clientFactory, ICache internalCache)
        {
            _serviceEndpoint = serviceEndpoint;
            _clusterName = clusterName;
            _appName = appName;
            _appSecret = appSecret;
            _preferredLocation = preferredLocation;
            _httpClientFactory = clientFactory;
            _internalCache = internalCache;
        }

        public UnifiedRedisPlatformServiceClient(string serviceEndpoint, string clusterName, string appName, string appSecret, string preferredLocation)
            : this(serviceEndpoint, clusterName, appName, appSecret, preferredLocation, new HttpClientFactory(maxRetry: 10, backOffInterval: 2500), new InternalCache())
        { }

        public async Task<AuthTokenResponseModel> GetAuthToken()
        {
            var cachedTokenKey = $"Auth:Token:{_clusterName}:{_appName}";
            var cachedToken = _internalCache.Get<AuthTokenResponseModel>(cachedTokenKey);
            if (cachedToken != null)
                return cachedToken;

            var client = _httpClientFactory.GetUnifiedPlatformClient();
            var authEndpoint = $"{_serviceEndpoint}/api/token";

            var request = new HttpRequestMessage(HttpMethod.Post, authEndpoint);
            request.Headers.Add("x-URP-secret", _appSecret);
            if (!string.IsNullOrWhiteSpace(_preferredLocation))
                request.Headers.Add("x-location", _preferredLocation);

            var content = new AuthTokenRequestModel(_clusterName, _appName);
            request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            using (var response = await client.SendAsync(request).ConfigureAwait(false))
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new UnauthorizedException(Guid.NewGuid().ToString());
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new InvalidConfigurationException("AppName, ClusterName", new Exception(message));
                }

                var result = await response.Content.ReadAsStringAsync();
                var authResponse = JsonConvert.DeserializeObject<AuthTokenResponseModel>(result);

                var tokenExpiresOn = new DateTime(authResponse.ExpiresOn);
                var safeTokenExpirationDate = tokenExpiresOn.AddMinutes(-10);
                _internalCache.Set(cachedTokenKey, authResponse, safeTokenExpirationDate);

                return authResponse;
            }
        }

        public async Task<ClusterConfiguration> GetClusterConfiguration()
        {
            var client = _httpClientFactory.GetUnifiedPlatformClient();
            var authResult = 
                await GetAuthToken()
                .ConfigureAwait(false);
            var configurationEndpoint = $"{_serviceEndpoint}/api/configurations";

            var request = new HttpRequestMessage(HttpMethod.Get, configurationEndpoint);
            request.Headers.Add("Authorization", $"Bearer {authResult.Token}");
            if (!string.IsNullOrWhiteSpace(_preferredLocation))
                request.Headers.Add("x-location", _preferredLocation);

            using (var response = await client.SendAsync(request).ConfigureAwait(false))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Application hasn't been registered, please contact support");
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new InvalidConfigurationException("AppName, ClusterName", new Exception(message));
                }
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var configuration = JsonConvert.DeserializeObject<ClusterConfiguration>(responseContent);
                configuration.AddDefaultValues();
                return configuration;
            }
        }

        public async Task<ServerLogResult> Log(List<GenericLog> logs, int attempt, int maxAttempt)
        {
            if (attempt > maxAttempt)
            {
                return new ServerLogResult()
                {
                    FailedLogs = logs,
                    LogsFailed = logs.Count,
                    LogsReceived = logs.Count
                };
            }

            var client = _httpClientFactory.GetUnifiedPlatformClient();
            var authResult = await GetAuthToken().ConfigureAwait(false);
            var logEndpoint = $"{_serviceEndpoint}/api/logs";

            var request = new HttpRequestMessage(HttpMethod.Post, logEndpoint);
            request.Headers.Add("Authorization", $"Bearer {authResult.Token}");
            request.Content = new StringContent(JsonConvert.SerializeObject(logs), Encoding.UTF8, "application/json");

            using (var response = await client.SendAsync(request))
            {
                if (!response.IsSuccessStatusCode)
                    return await Log(logs, attempt + 1, maxAttempt);

                var responseContent = await response.Content.ReadAsStringAsync();
                var serverResult = JsonConvert.DeserializeObject<ServerLogResult>(responseContent);

                if (serverResult.LogsFailed == 0)
                    return serverResult;

                return await Log(serverResult.FailedLogs, attempt + 1, maxAttempt);
            }
        }
    }
}
