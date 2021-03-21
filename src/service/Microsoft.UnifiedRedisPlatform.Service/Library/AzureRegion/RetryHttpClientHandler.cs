using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AzureRegion
{
    internal class RetryHttpClientHandler: DelegatingHandler
    {

        private readonly int _maxRetryAttempts;
        public readonly int _backoffInterval;

        public RetryHttpClientHandler(HttpMessageHandler innerHandler, int maxRetryAttempts = 10, int backoffInterval = 1000)
            : base(innerHandler)
        {
            _maxRetryAttempts = maxRetryAttempts;
            _backoffInterval = backoffInterval;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (var httpIterator = 0; httpIterator < _maxRetryAttempts; httpIterator++)
            {
                response = await base.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode && !IsTransientFailure(response))
                {   
                    await Task.Delay(_backoffInterval);
                }
                else
                {
                    return response;
                }
            }
            return response;
        }


        private bool IsTransientFailure(HttpResponseMessage response)
        {
            return (int)response.StatusCode >= 400 && (int)response.StatusCode < 500;
        }
    }
}
