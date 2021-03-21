using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.UnifiedPlatform.Service.Common.Storage;
using Microsoft.UnifiedPlatform.Service.Common.Caching;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Configuration.Providers
{
    public class StorageConfigurationProvider : BaseConfigurationProvider
    {
        private readonly int _priority;
        public override int Priority => _priority;

        private readonly bool _emptyCachingEnabled;
        private readonly ITableReader<ConfigurationEntity> _configurationTableReader;
        private readonly IBlobReader _blobReader;
        private readonly ICacheService _cacheService;

        public StorageConfigurationProvider(ITableReader<ConfigurationEntity> configurationTableReader, IBlobReader blobReader, ICacheService cacheService, bool emptyCachingEnabled = true, int priority = 2)
        {
            _configurationTableReader = configurationTableReader;
            _blobReader = blobReader;
            _cacheService = cacheService;
            _emptyCachingEnabled = emptyCachingEnabled;
            _priority = priority;
        }

        protected override async Task<string> HandleConfigurationRequest(string feature, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return await GetConfigurationByFeature(feature);
            return await GetConfigurationByFeatureAndKey(feature, key);
        }

        private async Task<string> GetConfigurationByFeatureAndKey(string feature, string key)
        {
            var configurationKey = $"STG:{feature}:{key}";
            var cachedConfiguration = await _cacheService.Get<string>(configurationKey);
            if (!string.IsNullOrWhiteSpace(cachedConfiguration))
            {
                if (_emptyCachingEnabled && cachedConfiguration == "EMPTY:CACHE")
                    return null;
                return cachedConfiguration;
            }

            var configurationEntity = await _configurationTableReader.Get(feature, key, correlationId: string.Empty, transactionId: string.Empty);
            if (configurationEntity == null)
            {
                if (_emptyCachingEnabled)
                    await _cacheService.Set(configurationKey, "EMPTY:CACHE");
                return null;
            }

            if (!configurationEntity.IsBlob)
            {
                var configuredValue = configurationEntity.Value;
                await _cacheService.Set(configurationKey, configuredValue);
                return configuredValue;
            }

            var blobConfigurationValue = await GetConfigurationFromBlob(configurationEntity);
            return blobConfigurationValue;
        }

        private async Task<string> GetConfigurationByFeature(string feature)
        {
            var configurationKey = $"STG:{feature}";
            var cachedConfiguration = await _cacheService.Get<string>(configurationKey);
            if (!string.IsNullOrWhiteSpace(cachedConfiguration))
            {
                if (_emptyCachingEnabled && cachedConfiguration == "EMPTY:CACHE")
                    return null;
                return cachedConfiguration;
            }

            var configurationEntities = await _configurationTableReader.QueryAll(feature, correlationId: string.Empty, transactionId: string.Empty);

            if (configurationEntities == null || !configurationEntities.Any())
            {
                if (_emptyCachingEnabled)
                    await _cacheService.Set(configurationKey, "EMPTY:CACHE");
                return null;
            }

            var configurationDictionary = new ConcurrentDictionary<string, string>();
            var getConfigurationTasks = new List<Task>();
            foreach(var configurationEntity in configurationEntities)
            {
                getConfigurationTasks.Add(Task.Run(async () =>
                {
                    var blobConfigurationValue = await GetConfigurationFromBlob(configurationEntity);
                    configurationDictionary.AddOrUpdate(configurationEntity.RowKey, blobConfigurationValue);
                }));
            }
            await Task.WhenAll(getConfigurationTasks);
            if (!configurationDictionary.Any() && _emptyCachingEnabled)
            {
                await _cacheService.Set(configurationKey, "EMPTY:CACHE");
            }

            return JsonConvert.SerializeObject(configurationDictionary);
        }
        
        private async Task<string> GetConfigurationFromBlob(ConfigurationEntity configurationEntity)
        {
            var configurationKey = $"STG:{configurationEntity.PartitionKey}:{configurationEntity.RowKey}";
            var splittedBlobUrl = configurationEntity.Value.Split('/');
            var containerName = splittedBlobUrl.First();
            var blobName = splittedBlobUrl.Last();

            var blobConfigurationValue = await _blobReader.Read(containerName, blobName, correlationId: string.Empty, transactionId: string.Empty);
            if (string.IsNullOrWhiteSpace(blobConfigurationValue))
            {
                if (_emptyCachingEnabled)
                    await _cacheService.Set(configurationKey, "EMPTY:CACHE");
                return null;
            }

            await _cacheService.Set(configurationKey, blobConfigurationValue);
            return blobConfigurationValue;
        }
    }
}
