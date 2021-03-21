using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UnifiedPlatform.Service.Common.Caching;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedPlatform.Service.Configuration.Providers
{
    public class FileBasedConfigurationProvider : BaseConfigurationProvider
    {
        private readonly ICacheService _cacheService;
        private readonly string _configurationFileDirectory;
        private readonly int _priority;
        private readonly bool _emptyCachingEnabled;

        public FileBasedConfigurationProvider(ICacheService cacheService, string configurationFileDirectory = null, int priority = 2, bool enableEmptyCaching = true)
        {
            _cacheService = cacheService;
            _configurationFileDirectory = string.IsNullOrWhiteSpace(configurationFileDirectory) ? AppDomain.CurrentDomain.BaseDirectory : configurationFileDirectory;
            _priority = priority;
            _emptyCachingEnabled = enableEmptyCaching;
        }

        public override int Priority => _priority;

        protected override async Task<string> HandleConfigurationRequest(string feature, string key)
        {
            var cacheKey = $"File:{feature}:{key}";
            var cachedConfiguration = await _cacheService.Get<string>(cacheKey);

            if (!string.IsNullOrWhiteSpace(cachedConfiguration))
            {
                if (_emptyCachingEnabled && cachedConfiguration == "EMPTY:CACHE")
                    return null;
                return cachedConfiguration;
            }

            var fileName = !string.IsNullOrWhiteSpace(key) ? $"{feature}-{key}.json" : $"{feature}.json";
            var filePath = $"{_configurationFileDirectory}{fileName}";
            if (!File.Exists(filePath))
            {
                if (_emptyCachingEnabled)
                    await _cacheService.Set(cacheKey, "EMPTY:CACHE");
                return null;
            }

            var configuration = File.ReadAllText(filePath);
            await _cacheService.Set(cacheKey, configuration);

            return configuration;
        }
    }
}
