using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.UnifiedPlatform.Service.Common.DefaultHttpClient
{
    public class RetryHandler: DelegatingHandler
    {
        private readonly RetryProtocol _retryProtocol;
        
        public RetryHandler(HttpMessageHandler innerHandler, RetryProtocol retryProtocol)
            : base(innerHandler)
        {
            _retryProtocol = retryProtocol;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (var httpIterator = 0; httpIterator < _retryProtocol.MaxRetryCount; httpIterator++)
            {
                response = await base.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode && !IsTransientFailure(response))
                {
                    LogFailedAttempt(httpIterator + 1, response);
                    await Task.Delay(_retryProtocol.MaxBackOffInterval);
                }
                else
                {
                    return response;
                }
            }
            return response;
        }

        private void LogFailedAttempt(int retryAttempt, HttpResponseMessage response)
        {
            // Log Here
        }

        private bool IsTransientFailure(HttpResponseMessage response)
        {
            if (_retryProtocol.TransientFailureCodes == null || _retryProtocol.TransientFailureCodes.Count == 0)
                return false;
            return _retryProtocol.TransientFailureCodes.Contains(response.StatusCode.ToString());
        }
    }
}
