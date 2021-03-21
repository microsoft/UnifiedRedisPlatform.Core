using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.UnifiedPlatform.Service.Common.Models;
using Microsoft.UnifiedPlatform.Service.Common.AppExceptions;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Configuration
{
    public class ClusterConfigurationProvider : IClusterConfigurationProvider
    {
        private readonly BaseConfigurationProvider _configurationProvider;

        public ClusterConfigurationProvider(BaseConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task<List<ClusterConfigurationDto>> GetAllClusters()
        {
            try
            {
                var clusterConfig = await _configurationProvider.GetConfiguration("Cluster", null);
                var clusters = JsonConvert.DeserializeObject<Dictionary<string, string>>(clusterConfig);
                if (clusters != null && clusters.Any())
                    return clusters.Values.Select(value => JsonConvert.DeserializeObject<ClusterConfigurationDto>(value)).ToList();
                return null;
            }
            catch (ConfigurationNotFoundException)
            {
                return null;
            }
        }

        public async Task<List<AppConfigurationDto>> GetAllApplications(string clusterName)
        {
            try
            {
                var applicationConfig = await _configurationProvider.GetConfiguration(clusterName, null);
                var applications = JsonConvert.DeserializeObject<Dictionary<string, string>>(applicationConfig);
                if (applications != null && applications.Any())
                    return applications.Values.Select(value => JsonConvert.DeserializeObject<AppConfigurationDto>(value)).ToList();
                return null;
            }
            catch (ConfigurationNotFoundException)
            {
                return null;
            }
        }

        public async Task<ClusterConfigurationDto> GetClusterDetails(string clusterName)
        {
            try
            {
                var clusterDetails = await _configurationProvider.GetConfiguration("Cluster", clusterName);
                return JsonConvert.DeserializeObject<ClusterConfigurationDto>(clusterDetails);
            }
            catch (ConfigurationNotFoundException exception)
            {
                throw new InvalidClusterException(clusterName, exception);
            }
        }

        public async Task<AppConfigurationDto> GetApplicationDetails(string clusterName, string appName)
        {
            try
            {
                var appDetails = await _configurationProvider.GetConfiguration(clusterName, appName);
                return JsonConvert.DeserializeObject<AppConfigurationDto>(appDetails);
            }
            catch (ConfigurationNotFoundException exception)
            {
                throw new InvalidAppException(clusterName, appName, exception);
            }
        }

        public async Task<string> GetApplicationSecret(string clusterName, string appName)
        {
            await GetApplicationDetails(clusterName, appName);
            var appSecretKey = $"{clusterName}-{appName}";
            try
            {
                var appSecret = await _configurationProvider.GetConfiguration(appSecretKey, "Secret");
                return appSecret;
            }
            catch (ConfigurationNotFoundException exception)
            {
                throw new IncompleteConfigurationException(clusterName, appName, $"ApplicationSecret: {appSecretKey}-Secret", exception);
            }
        }

        public async Task<List<ConnectionStringDto>> GetClusterConnectionStrings(string clusterName, string appName)
        {
            var clusterDetails = await GetClusterDetails(clusterName);
            try
            {
                if (clusterDetails.ConnectionStrings == null || !clusterDetails.ConnectionStrings.Any())
                {
                    var redisConnectionString = await _configurationProvider.GetConfiguration(clusterName, "Redis-ConnectionString");
                    return new List<ConnectionStringDto>()
                    {
                        new ConnectionStringDto()
                        {
                            ConnectionString = redisConnectionString
                        }
                    };
                }
                else
                {
                    var connectionStrings = new ConcurrentBag<ConnectionStringDto>();
                    var connectionStringFetchTasks = new List<Task>();

                    foreach (var connectionStringConfig in clusterDetails.ConnectionStrings)
                    {
                        var fetchConnectionStringTask = Task.Run(async () =>
                        {
                            var region = connectionStringConfig.Region?.Name;
                            var connectionStringKey = string.IsNullOrWhiteSpace(region) ? connectionStringConfig.ConnectionStringLocation : $"Redis-ConnectionString-{region}";
                            var connectionString = await _configurationProvider.GetConfiguration(clusterName, connectionStringKey);
                            var connectionStringDto = new ConnectionStringDto()
                            {
                                ConnectionString = connectionString,
                                IsPrimary = connectionStringConfig.IsPrimary,
                                IsWriteEnabled = connectionStringConfig.IsWriteEnabled,
                                Region = connectionStringConfig.Region
                            };
                            connectionStrings.Add(connectionStringDto);
                        });
                        connectionStringFetchTasks.Add(fetchConnectionStringTask);
                    }

                    await Task.WhenAll(connectionStringFetchTasks);

                    return connectionStrings.ToList();
                }
            }
            catch (ConfigurationNotFoundException exception)
            {
                throw new IncompleteConfigurationException(clusterName, appName, $"RedisConnectionString: {clusterName}-Redis-ConnectionString", exception);
            }
        }
    }
}
