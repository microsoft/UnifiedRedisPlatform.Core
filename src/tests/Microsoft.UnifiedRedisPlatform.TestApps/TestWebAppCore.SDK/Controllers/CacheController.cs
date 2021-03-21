using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.TestWebAppCore.SDK.Controllers
{
    [ApiController]
    [Route("api/cache")]
    public class CacheController : ControllerBase
    {
        private readonly IDistributedCache _cache;

        public CacheController(IDistributedCache cache)
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
    }
}
