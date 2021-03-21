using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.UnifiedRedisPlatform.FunctionalTests;

namespace FunctionalTests
{
    // You need to keep password in the FunctionalTestSettings.runsetitngs to run the test
    [TestClass]
    public class ConfigurationTests
    {
        private static TestContext _testContext;
        private static UnifiedRedisClient _client;

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            _testContext = testContext;
            _client = new UnifiedRedisClient(testContext);
        }

        [TestMethod]
        public async Task Should_Get_Configurations_For_All_Registered_Apps()
        {
            // Arrange
            Dictionary<string, List<string>> registeredApplications = GetRegisteredApplications();
            List<string> regions = GetRegions();
            List<Task> testTasks = new List<Task>();

            // Assert
            foreach (var region in regions)
            {
                foreach (var clusteredApps in registeredApplications)
                {
                    foreach (var application in clusteredApps.Value)
                    {
                        testTasks.Add(Task.Run(async () =>
                        {
                            string cluster = clusteredApps.Key;
                            ConfigurationResponse configuration = await GetConfiguration(cluster, application, region);
                            
                            // Assert
                            AssertConfiguration(configuration, cluster, application, region);
                        }));
                    }
                }
            }

            await Task.WhenAll(testTasks);
        }

        private Dictionary<string, List<string>> GetRegisteredApplications()
        {
            Dictionary<string, List<string>> registeredApplications = new Dictionary<string, List<string>>();
            List<string> clusters = _testContext.Properties["FunctionalTest:Clusters"].ToString().Split(',').ToList();
            foreach (var cluster in clusters)
            {
                List<string> clusterApplications = _testContext.Properties[$"FunctionalTest:{cluster}:Apps"].ToString().Split(',').ToList();
                registeredApplications.Add(cluster, clusterApplications);
            }
            return registeredApplications;
        }

        private List<string> GetRegions()
        {
            List<string> regions = _testContext.Properties["FunctionalTest:Regions"].ToString().Split(',').ToList();
            regions.Add(string.Empty); // For auto-load balancing
            return regions;
        }

        private Task<ConfigurationResponse> GetConfiguration(string clusterName, string appName, string region)
        {
            string password = _testContext.Properties[$"FunctionalTest:{clusterName}:{appName}:Password"].ToString();
            return _client.GetConfiguration(clusterName, appName, password, region);
        }

        private void AssertConfiguration(ConfigurationResponse configuration, string cluster, string appName, string region)
        {
            string genericFailureMessage = $"Assert failed for {cluster}:{appName}. Region - {region}";
            Assert.IsNotNull(configuration, genericFailureMessage);
            Assert.AreEqual(cluster, configuration.ClusterName, genericFailureMessage);
            Assert.IsNotNull(configuration.RedisCachePrefix, genericFailureMessage);
            if (!string.IsNullOrWhiteSpace(region) && !string.IsNullOrWhiteSpace(configuration.PrimaryRedisRegion))
                Assert.AreEqual(region, configuration.PrimaryRedisRegion, genericFailureMessage);


            Assert.IsNotNull(configuration.Applications, genericFailureMessage);
            Assert.AreEqual(1, configuration.Applications.Count, genericFailureMessage);

            ApplicationConfiguration appConfiguration = configuration.Applications[0];
            Assert.IsNotNull(appConfiguration, genericFailureMessage);
            Assert.AreEqual(appName, appConfiguration.AppName, genericFailureMessage);
            Assert.IsNotNull(appConfiguration.RedisCachePrefix, genericFailureMessage);
            Assert.IsNotNull(appConfiguration.ConnectionPreference.ConnectionRetryProtocol, genericFailureMessage);
            Assert.IsNotNull(appConfiguration.ConnectionPreference.OperationalRetryProtocol, genericFailureMessage);
            Assert.IsNotNull(appConfiguration.DiagnosticSettings, genericFailureMessage);
        }
    }
}
