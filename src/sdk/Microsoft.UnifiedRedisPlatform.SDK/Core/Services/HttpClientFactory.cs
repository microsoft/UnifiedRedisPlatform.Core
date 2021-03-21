using System.Net.Http;
using Microsoft.UnifiedRedisPlatform.Core.Services.Interfaces;

namespace Microsoft.UnifiedRedisPlatform.Core.Services
{
    internal class HttpClientFactory: IHttpClientFactory
    {
        private static HttpClient _serviceClient;
        private static readonly object _lock = new object();

        private readonly int _maxRetry;
        private readonly int _backoffInterval;

        public HttpClientFactory(int maxRetry, int backOffInterval)
        {
            _maxRetry = maxRetry;
            _backoffInterval = backOffInterval;
        }

        public HttpClient GetUnifiedPlatformClient()
        {
            lock (_lock)
            {
                if (_serviceClient == null)
                {
                    var retryHandler = new RetryHttpHandler(new HttpClientHandler(), _maxRetry, _backoffInterval);
                    _serviceClient = new HttpClient(retryHandler);
                }
            }
            return _serviceClient;
        }
    }
}
