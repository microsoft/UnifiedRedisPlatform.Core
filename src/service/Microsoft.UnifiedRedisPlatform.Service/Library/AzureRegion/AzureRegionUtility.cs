using GeoCoordinatePortable;
using Microsoft.AzureRegion.Models;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.AzureRegion
{
    public class AzureRegionUtility : IAzureRegionUtility
    {
        public string AzureSubscriptionId { get; set; }
        public string AzureManagementEndpoint { get; set; }
        public string AzureManagementAadResourceId { get; set; }
        public string AadAuthority { get; set; }
        public string AadClientId { get; set; }
        public int CacheDurationInMins { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;

        private static List<RegionModel> _cachedAzureRegions = new List<RegionModel>();
        private static DateTime _cachedUntil = DateTime.UtcNow;
        private readonly ConcurrentDictionary<string, IConfidentialClientApplication> confidentialApps = new ConcurrentDictionary<string, IConfidentialClientApplication>();
        private readonly string CertificateThumbprint;

        public AzureRegionUtility(string certificateThumbprint)
            : this(azureSubscriptionId: "05a315f7-744f-4692-b9dd-1aed7c6cee64",
                 azureManagementEndpoint: "https://management.azure.com/subscriptions/{0}/locations?api-version=2019-06-01",
                 azureAadResourceId: "https://management.core.windows.net/",
                 aadAuthority: "https://login.microsoftonline.com/microsoft.onmicrosoft.com",
                 aadClientId: "1601a33e-356e-4570-8325-eefe6116eadb",
                 cacheDurationInMins: 43200,
                 certificateThumbprint: certificateThumbprint
                 )
        { }

        public AzureRegionUtility(string azureSubscriptionId, string azureManagementEndpoint, string azureAadResourceId, string aadAuthority, string aadClientId, int cacheDurationInMins, string certificateThumbprint)
            : this(azureSubscriptionId, azureManagementEndpoint, azureAadResourceId, aadAuthority, aadClientId, cacheDurationInMins, certificateThumbprint, new HttpClientFactory())
        {
        }

        internal AzureRegionUtility(string azureSubscriptionId, string azureManagementEndpoint, string azureAadResourceId, string aadAuthority, string aadClientId, int cacheDurationInMins, string certificateThumbprint, IHttpClientFactory clientFactory)
        {
            AzureSubscriptionId = azureSubscriptionId;
            AzureManagementEndpoint = azureManagementEndpoint;
            AzureManagementAadResourceId = azureAadResourceId;
            AadAuthority = aadAuthority;
            AadClientId = aadClientId;
            CacheDurationInMins = cacheDurationInMins;
            CertificateThumbprint = certificateThumbprint;
            _httpClientFactory = clientFactory;
        }

        public async Task<List<RegionModel>> GetAllAzureRegions(bool useInMemCache = true)
        {
            if (useInMemCache)
            {
                if (_cachedAzureRegions != null && _cachedAzureRegions.Any() && _cachedUntil >= DateTime.UtcNow)
                    return _cachedAzureRegions;
            }

            var url = string.Format(AzureManagementEndpoint, AzureSubscriptionId);
            var httpClient = _httpClientFactory.Create();

            var authToken = await GenerateAuthToken();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {authToken}");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var rawResponse = await response.Content.ReadAsStringAsync();

            var azureLocationResponse = JsonConvert.DeserializeObject<AzureLocationApiResponse>(rawResponse);
            var azureRegions = azureLocationResponse.Value;

            _cachedAzureRegions = azureRegions;
            _cachedUntil = DateTime.UtcNow.AddMinutes(CacheDurationInMins);

            return _cachedAzureRegions;
        }

        public async Task<bool> Exists(string regionName)
        {
            var regions = await GetAllAzureRegions();
            return regions.Any(region => region.Name == regionName);
        }

        public async Task<RegionModel> GetNearestRegion(string regionName, List<string> availableRegionNames)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                throw new ArgumentNullException(nameof(regionName));

            var regions = await GetAllAzureRegions();
            var currentRegion = regions.FirstOrDefault(region => region.Name == regionName);
            if (currentRegion == null)
                throw new ArgumentException("No datacenter found in the given region", nameof(regionName));

            var availableRegions = availableRegionNames.Select(availableRegionName => regions.FirstOrDefault(region => region.Name == availableRegionName)).ToList();
            return GetNearestRegion(currentRegion, availableRegions);
        }

        public async Task<RegionModel> GetNearestRegion(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                throw new ArgumentNullException(nameof(regionName));

            var regions = await GetAllAzureRegions();
            var currentRegion = regions.FirstOrDefault(region => region.Name == regionName);
            if (currentRegion == null)
                throw new ArgumentException("No datacenter found in the given region", nameof(regionName));

            return GetNearestRegion(currentRegion, regions);
        }

        private RegionModel GetNearestRegion(RegionModel currentRegion, List<RegionModel> availableRegions)
        {
            var regionGeoCoord = new GeoCoordinate(currentRegion.Latitude, currentRegion.Longitude);
            var nearestRegion = availableRegions
                .OrderBy(region =>
                {
                    var regionCoord = new GeoCoordinate(region.Latitude, region.Longitude);
                    return regionCoord.GetDistanceTo(regionGeoCoord);
                })
                .First();
            return nearestRegion;
        }

        private async Task<string> GenerateAuthToken()
        {
            try
            {
                IConfidentialClientApplication app = GetOrCreateConfidentialApp(AadAuthority, AadClientId);

                var authResult = await app.AcquireTokenForClient(new[] { $"{AzureManagementAadResourceId}/.default" }).ExecuteAsync();
                return authResult.AccessToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IConfidentialClientApplication GetOrCreateConfidentialApp(string authority, string clientId)
        {
            try
            {
                string confidentialAppCacheKey = CreateConfidentialAppCacheKey(authority, clientId);
                if (confidentialApps.ContainsKey(confidentialAppCacheKey))
                {
                    return confidentialApps[confidentialAppCacheKey];
                }
#if DEBUG
                var certificate = GetCertificate(CertificateThumbprint);

                IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(clientId)
                                                                                .WithAuthority(authority)
                                                                                .WithCertificate(certificate, true)
                                                                                .Build();
                confidentialApps.TryAdd(confidentialAppCacheKey, confidentialClientApplication);
                return confidentialClientApplication;
#else
                IConfidentialClientApplication app =  ConfidentialClientApplicationBuilder
                                                .Create(clientId)
                                                .WithAuthority(new Uri(authority))
                                                .WithClientAssertion(new ManagedIdentityClientAssertion(clientId).GetSignedAssertion).Build();
                confidentialApps.TryAdd(confidentialAppCacheKey, app);
                return app;
#endif
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string CreateConfidentialAppCacheKey(string authority, string clientId)
        {
            return $"{authority}-{clientId}";
        }

        public X509Certificate2 GetCertificate(string certificateThumbprint)
        {
            using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                var cert = store.Certificates.OfType<X509Certificate2>()
                    .FirstOrDefault(x => x.Thumbprint?.ToUpper() == certificateThumbprint?.ToUpper());
                return cert;
            }
        }
    }
}
