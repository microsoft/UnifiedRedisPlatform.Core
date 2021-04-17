using System;
using System.Linq;
using System.Text;
using StackExchange.Redis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.UnifiedRedisPlatform;

namespace Microsoft.UnifiedRedisPlatform.TestWebAppCore.SDK.Controllers
{
    [ApiController]
    [Route("api/cache")]
    public class CacheController : ControllerBase
    {
        private readonly IDistributedUnifiedRedisCache _cache;

        public CacheController(IDistributedUnifiedRedisCache cache)
        {
            _cache = cache;
        }

        [HttpPost]
        [Route("keys")]
        public async Task<IActionResult> Set([FromBody]CachedObject obj)
        {
            if (obj.Options == null)
                await _cache.SetAsync(obj.Key, Encoding.ASCII.GetBytes(obj.Value));
            else
                await _cache.SetStringAsync(obj.Key, obj.Value, new DistributedCacheEntryOptions 
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(obj.Options.AbsoluteExpiration),
                        SlidingExpiration = TimeSpan.FromSeconds(obj.Options.SlidingWindow)
                    });
            return new CreatedResult("Get", obj.Key);
        }

        [HttpGet]
        [Route("keys/{key}")]
        public async Task<IActionResult> Get([FromRoute] string key)
        {
            var value = await _cache.GetAsync(key);
            if (value == null)
                return new NotFoundResult();
            return new OkObjectResult(Encoding.ASCII.GetString(value));
        }

        [HttpGet]
        [Route("keys/{pattern}/search")]
        public async Task<IActionResult> Search([FromRoute] string pattern)
        {
            IEnumerable<RedisKey> keys = await _cache.GetKeys(pattern);
            if (keys == null || !keys.Any())
                return new NotFoundResult();
            return new OkObjectResult(keys.Select(key => key.ToString()));
        }
    }
}
