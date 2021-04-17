using System.Net.Http;

namespace Microsoft.UnifiedRedisPlatform.Core.Services.Interfaces
{
    internal interface IHttpClientFactory
    {
        HttpClient GetUnifiedPlatformClient();
    }
}
