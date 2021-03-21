using System.Net.Http;

namespace Microsoft.UnifiedPlatform.Service.Common.DefaultHttpClient
{
    public interface IHttpClientFactory
    {
        HttpClient CreateClient(string name, RetryProtocol retryProtocol);
    }
}