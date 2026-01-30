using Microsoft.Extensions.Options;
using Microsoft.UnifiedRedisPlatform.Core;

namespace Microsoft.Extensions.Caching.UnifiedRedisPlatform
{
    /// <summary>
    /// Configuration options for <see cref="UnifiedRedisCache"/>
    /// </summary>
    public class UnifedRedisPlatformOptions : IOptions<UnifedRedisPlatformOptions>
    {
        /// <summary>
        /// ID of the URP cluster
        /// </summary>
        public string Cluster { get; set; }
        
        /// <summary>
        /// Application name in the above cluster
        /// </summary>
        public string Application { get; set; }
        
        /// <summary>
        /// Application secret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// The URP service endpoint URL. Defaults to production endpoint if not specified.
        /// </summary>
        public string ServiceEndpoint { get; set; }

        /// <summary>
        /// Preferred location of the primary Redis server
        /// </summary>
        public string PreferredLocation { get; set; }

        /// <summary>
        /// The client ID of the User-Assigned Managed Identity to use for Redis authentication.
        /// Leave null/empty to use access key authentication (backward compatible).
        /// </summary>
        public string ManagedIdentityClientId { get; set; }

        /// <summary>
        /// Configuration options for connecting to Unified Redis Cluster
        /// </summary>
        public UnifiedConfigurationServerOptions ConfigurationOptions { get; set; }

        public UnifedRedisPlatformOptions Value => this;
    }
}
