using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core.Services
{
    internal class RetryHttpHandler : DelegatingHandler
    {
        private readonly int _maxRetry;
        private readonly int _backoffInterval;

        public RetryHttpHandler(HttpMessageHandler innerHandler, int maxRetry, int backOffInterval)
            :base(innerHandler)
        {
            _maxRetry = maxRetry;
            _backoffInterval = backOffInterval;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (var httpIterator = 0; httpIterator < _maxRetry; httpIterator++)
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!IsResponseSuccess(response))
                    await Task.Delay(_backoffInterval);
                else
                    return response;
            }
            return response;
        }

        private bool IsResponseSuccess(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return true;

            return (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Unauthorized);
        }
    }
}
