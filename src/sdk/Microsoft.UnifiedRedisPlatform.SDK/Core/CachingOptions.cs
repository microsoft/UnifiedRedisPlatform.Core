using System;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    /// <summary>
    /// Additional options while setting up a cache key
    /// </summary>
    public class CachingOptions
    {
        public const short NeverExpire = -1;
        public CachingOptions()
        {
            AbsoluteExpiration = null;
            RelativeAbsoluteExpiration = null;
            SlidingWindow = null;
        }
    
        /// <summary>
        /// Time when the current cache will expire
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        
        /// <summary>
        /// Given Time + Current Time when the cache will expire
        /// </summary>
        public TimeSpan? RelativeAbsoluteExpiration { get; set; }
        
        /// <summary>
        /// Inactive period after which the cache will be removed. Sliding cannot be extended beyond the Absolute expiration period
        /// </summary>
        public TimeSpan? SlidingWindow { get; set; }

        public long GetAbsoluteExpirationTicks(DateTimeOffset createdOn)
        {   

            if (AbsoluteExpiration != null && AbsoluteExpiration.HasValue)
                return AbsoluteExpiration.Value.Ticks;

            if (RelativeAbsoluteExpiration != null && RelativeAbsoluteExpiration.HasValue)
                return (createdOn + RelativeAbsoluteExpiration.Value).Ticks;

            return NeverExpire;
        }

        public long GetSlidingWindowExpirationTicks()
        {   
            if (SlidingWindow == null || !SlidingWindow.HasValue)
                return NeverExpire;

            return SlidingWindow.Value.Ticks;
        }
    }
}
