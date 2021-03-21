using System;

namespace Microsoft.UnifiedRedisPlatform.Core.Utilities
{
    /// <summary>
    /// Utility class for calculating Expiration time for a key
    /// </summary>
    public static class ExpirationTimeCalculator
    {
        /// <summary>
        /// Calculates the expiration time between 2 timespans. If new one is present then new time is returned.
        /// </summary>
        /// <param name="originalExpirationTime">Original expiration time set for the key</param>
        /// <param name="newExpirationTime">New expiration time to be set for the key</param>
        /// <returns cref="TimeSpan?">New timespan if present, else the existing TimeSpan and NULL if neither is present</returns>
        public static TimeSpan? GetExpirationTime(TimeSpan? originalExpirationTime, TimeSpan? newExpirationTime)
        {
            if (newExpirationTime != null && newExpirationTime.HasValue)
                return newExpirationTime;

            if (originalExpirationTime != null && originalExpirationTime.HasValue)
                return originalExpirationTime;

            return null;
        }

        /// <summary>
        /// Calculates the expiration between when the ticks for aboslute expiration value and sliding window is provided
        /// </summary>
        /// <param name="absoluteExpirationTicks">Ticks representing the absolute expiration time</param>
        /// <param name="slidingWindowTicks">Ticks representing the sliding window</param>
        /// <returns cref="TimeSpan">If sliding window expired faster then window, else the aboslute expiration time is returned</returns>
        public static TimeSpan? GetExpirationTime(long absoluteExpirationTicks, long slidingWindowTicks)
        {
            DateTimeOffset? absoluteExpiration = null;
            if (absoluteExpirationTicks != CachingOptions.NeverExpire)
                absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks, TimeSpan.Zero);

            TimeSpan? slidingWindow = null;
            if (slidingWindowTicks != CachingOptions.NeverExpire)
                slidingWindow = new TimeSpan(slidingWindowTicks);

            return GetExpirationTime(absoluteExpiration, slidingWindow);
        }

        /// <summary>
        /// Calculates the expiration time from Cached Options by comparing the absolute expiration time and relative expiration time
        /// </summary>
        /// <param name="createdOn" cref="DateTimeOffset">Exact date time when the </param>
        /// <param name="options" cref="CachingOptions">Additional Options while setting up a key</param>
        /// <returns cref="TimeSpan">Absolute expiration or the relative expiration time, if absolute value is not present</returns>
        public static TimeSpan? GetExpirationTime(DateTimeOffset createdOn, CachingOptions options = null)
        {
            if (options == null)
                return null;

            DateTimeOffset? absoluteExpiration = null;
            var absoluteExpirationTicks = options.GetAbsoluteExpirationTicks(createdOn);
            if (absoluteExpirationTicks != CachingOptions.NeverExpire)
                absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks, TimeSpan.Zero);

            var expireOn = GetExpirationTime(absoluteExpiration, options.SlidingWindow);
            return expireOn;
        }

        /// <summary>
        /// Calculates the expiration time between the absolute expiration time and the sliding window value
        /// </summary>
        /// <param name="absoluteExpiration" cref="DateTimeOffset">Date Time when the key is supposed to expire</param>
        /// <param name="slidingWindow" cref="TimeSpan">Inactive period after which the key should be deleted</param>
        /// <returns cref="TimeSpan">If sliding window expired faster then window, else the aboslute expiration time is returned</returns>
        public static TimeSpan? GetExpirationTime(DateTimeOffset? absoluteExpiration, TimeSpan? slidingWindow)
        {
            TimeSpan? expireOn = null;
            if (absoluteExpiration != null && absoluteExpiration.HasValue)
            {
                expireOn = absoluteExpiration - DateTimeOffset.UtcNow;
            }

            if (slidingWindow != null && slidingWindow.HasValue)
            {
                expireOn = expireOn != null && expireOn.HasValue
                    ? (expireOn.Value > slidingWindow.Value ? slidingWindow : expireOn)
                    : slidingWindow;
            }
            return expireOn;
        }
    }
}
