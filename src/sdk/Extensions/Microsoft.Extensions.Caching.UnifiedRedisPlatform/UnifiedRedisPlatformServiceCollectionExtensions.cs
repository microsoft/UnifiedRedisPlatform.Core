using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Caching.UnifiedRedisPlatform
{
    public static class UnifiedRedisPlatformServiceCollectionExtensions
    {
        public static IServiceCollection AddUnifiedRedisPlatform(this IServiceCollection services, Action<UnifedRedisPlatformOptions> setupAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            services.AddOptions();
            services.Configure(setupAction);
            services.AddSingleton<IDistributedCache, UnifiedRedisCache>();

            return services;
        }
    }
}
