using System;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core
{
    public interface IUnifiedDatabase: IDatabase
    {
        /// <summary>
        /// Gets the value of a key. If the key doesn't exist or has been expired then NULL is returned.
        /// </summary>
        /// <param name="key">The key of the cached object</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of the key, NULL if it doesn't exist</returns>
        byte[] Get(RedisKey key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Gets the value of a key. If the key doesn't exist or has been expired then NULL is returned.
        /// </summary>
        /// <param name="key">The key of the cached object</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>The value of the key, NULL if it doesn't exist</returns>
        Task<byte[]> GetAsync(RedisKey key, CommandFlags flags = CommandFlags.None);
        
        /// <summary>
        /// Set key to hold the value of the given object. If key already exists then the value in overwritten. Keys can be set with an option containing expiration time and sliding window values
        /// </summary>
        /// <param name="key">The value of the cached object</param>
        /// <param name="value">The object to be cached</param>
        /// <param name="options" cref="CachingOptions">The options to be set for the key</param>
        /// <param name="flags">The flags to use for this operation</param>
        void Set(RedisKey key, byte[] value, CachingOptions options = null, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Set key to hold the value of the given object. If key already exists then the value in overwritten. Keys can be set with an option containing expiration time and sliding window values
        /// </summary>
        /// <param name="key">The key of the cached object</param>
        /// <param name="value">The object to be cached</param>
        /// <param name="options" cref="CachingOptions">The options to be set for the key</param>
        /// <param name="flags">The flags to use for this operation</param>
        Task SetAsync(RedisKey key, byte[] value, CachingOptions options = null, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Refreshes the sliding window expiration time for the key. If the key doesn't have a sliding window then no action is taken.
        /// If new sliding window value is sent, then previous sliding window value is overwritten
        /// </summary>
        /// <param name="key">The key of the cached object</param>
        /// <param name="newSlidingWindowExpiration">New sliding window for the key</param>
        /// <param name="flags">The flags to use for this operation</param>
        void ResetWindow(RedisKey key, TimeSpan? newSlidingWindowExpiration = null, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Refreshes the sliding window expiration time for the key. If the key doesn't have a sliding window then no action is taken.
        /// If new sliding window value is sent, then previous sliding window value is overwritten
        /// </summary>
        /// <param name="key">The key of the cached object</param>
        /// <param name="newSlidingWindowExpiration">New sliding window for the key</param>
        /// <param name="flags">The flags to use for this operation</param>
        Task ResetWindowAsync(RedisKey key, TimeSpan? newSlidingWindowExpiration = null, CommandFlags flags = CommandFlags.None);
    }
}
