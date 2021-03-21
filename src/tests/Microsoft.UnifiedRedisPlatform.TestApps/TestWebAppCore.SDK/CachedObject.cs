namespace Microsoft.UnifiedRedisPlatform.TestWebAppCore.SDK
{
    public class CachedObject
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public CacheSettings Options { get; set; }
    }

    public class CacheSettings
    {
        public int AbsoluteExpiration { get; set; }
        public int SlidingWindow { get; set; }
    }
    
}
