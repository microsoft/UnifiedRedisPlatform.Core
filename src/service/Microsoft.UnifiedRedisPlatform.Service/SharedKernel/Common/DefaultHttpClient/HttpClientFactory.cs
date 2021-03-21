using System.Net.Http;
using System.Collections.Concurrent;

namespace Microsoft.UnifiedPlatform.Service.Common.DefaultHttpClient
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private static readonly ConcurrentDictionary<string, HttpClient> Clients = new ConcurrentDictionary<string, HttpClient>();
        public HttpClient CreateClient(string name, RetryProtocol retryProtocol)
        {
            var retryHandler = new RetryHandler(new HttpClientHandler(), retryProtocol);
            return Clients.GetOrAdd(name, new HttpClient(retryHandler));
        }
    }
}
