using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using Microsoft.UnifiedRedisPlatform.Core.Utilities;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        private const string DataKey = "data";
        private const string ExpireOn = "expr";
        private const string SlidingWindowExpiration = "swexpr";

        public byte[] Get(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            var hashedValues = HashGet(key, new RedisValue[] { ExpireOn, SlidingWindowExpiration, DataKey }, flags);
            if (hashedValues == null || hashedValues.Length < 3)
                return null;

            ResetWindow(key, hashedValues);
            var cachedData = hashedValues[2];
            if (cachedData.IsNull)
                return null;
            return cachedData;
        }

        public async Task<byte[]> GetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            var hashedValues = await HashGetAsync(key, new RedisValue[] { ExpireOn, SlidingWindowExpiration, DataKey }, flags);
            if (hashedValues == null || hashedValues.Length < 3)
                return null;

            await ResetWindowAsync(key, hashedValues);
            var cachedData = hashedValues[2];
            if (cachedData.IsNull)
                return null;
            return cachedData;
        }

        public void Set(RedisKey key, byte[] value, CachingOptions options = null, CommandFlags flags = CommandFlags.None)
        {
            var creationTime = DateTimeOffset.UtcNow;
            var entries = CreateEntries(value, creationTime, options);
            HashSet(key, entries, flags);
            SetExpiration(key, creationTime, options);
        }

        public async Task SetAsync(RedisKey key, byte[] value, CachingOptions options = null, CommandFlags flags = CommandFlags.None)
        {
            var creationTime = DateTimeOffset.UtcNow;
            var entries = CreateEntries(value, creationTime, options);
            await HashSetAsync(key, entries, flags);
            await SetExpirationAsync(key, creationTime, options);
        }

        public void ResetWindow(RedisKey key, TimeSpan? newExpirationWindow = null, CommandFlags flags = CommandFlags.None)
        {
            var hashValues = HashGet(key, new RedisValue[] { ExpireOn, SlidingWindowExpiration });
            ResetWindow(key, hashValues, newExpirationWindow, flags);
        }

        public async Task ResetWindowAsync(RedisKey key, TimeSpan? newExpirationWindow = null, CommandFlags flags = CommandFlags.None)
        {
            var hashValues = await HashGetAsync(key, new RedisValue[] { ExpireOn, SlidingWindowExpiration });
            await ResetWindowAsync(key, hashValues, newExpirationWindow, flags);
        }

        private void ResetWindow(RedisKey key, RedisValue[] hashValues, TimeSpan? newExpirationWindow = null, CommandFlags flags = CommandFlags.None)
        {
            if (hashValues == null || hashValues.Length < 2)
                return;

            var expireOn = GetAdjustedExpirationTime(hashValues, newExpirationWindow);
            if (expireOn != null && expireOn.HasValue)
                KeyExpire(key, expireOn);

            if (newExpirationWindow != null && newExpirationWindow.HasValue)
                HashSet(key, SlidingWindowExpiration, newExpirationWindow.Value.Ticks, When.Always, flags);
        }

        private async Task ResetWindowAsync(RedisKey key, RedisValue[] hashValues, TimeSpan? newExpirationWindow = null, CommandFlags flags = CommandFlags.None)
        {
            if (hashValues == null || hashValues.Length < 2)
                return;

            var expireOn = GetAdjustedExpirationTime(hashValues, newExpirationWindow);
            if (expireOn != null && expireOn.HasValue)
                await KeyExpireAsync(key, expireOn);

            if (newExpirationWindow != null && newExpirationWindow.HasValue)
                await HashSetAsync(key, SlidingWindowExpiration, newExpirationWindow.Value.Ticks, When.Always, flags);
        }

        private TimeSpan? GetAdjustedExpirationTime(RedisValue[] hashValues, TimeSpan? newSlidingWindow)
        {
            var absoluteExpirationTicks = (long)hashValues[0];
            var slidingWindowExpirationTicks = (long)hashValues[1];

            var originalExpirationTime = ExpirationTimeCalculator.GetExpirationTime(absoluteExpirationTicks, slidingWindowExpirationTicks);
            var expireOn = ExpirationTimeCalculator.GetExpirationTime(newSlidingWindow, originalExpirationTime);
            return expireOn;
        }

        private HashEntry[] CreateEntries(byte[] value, DateTimeOffset creationTime, CachingOptions options = null)
        {
            return new HashEntry[]
            {
                new HashEntry(ExpireOn, options != null ? options.GetAbsoluteExpirationTicks(creationTime) : CachingOptions.NeverExpire),
                new HashEntry(SlidingWindowExpiration, options != null ? options.GetSlidingWindowExpirationTicks() : CachingOptions.NeverExpire),
                new HashEntry(DataKey, value)
            };
        }

        private void SetExpiration(RedisKey key, DateTimeOffset createdOn, CachingOptions options = null)
        {
            var expireOn = ExpirationTimeCalculator.GetExpirationTime(createdOn, options);
            if (expireOn != null && expireOn.HasValue)
                KeyExpire(key, expireOn, CommandFlags.FireAndForget);
        }

        private async Task SetExpirationAsync(RedisKey key, DateTimeOffset createdOn, CachingOptions options = null)
        {
            var expireOn = ExpirationTimeCalculator.GetExpirationTime(createdOn, options);
            if (expireOn != null && expireOn.HasValue)
                await KeyExpireAsync(key, expireOn, CommandFlags.FireAndForget);
        }
    }
}
