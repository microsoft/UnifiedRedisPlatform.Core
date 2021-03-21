using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.UnifiedPlatform.Service.Common.Configuration;

namespace Microsoft.UnifiedRedisPlatform.Service.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly AppMetadataConfiguration _configuration;
        public BaseController(AppMetadataConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected string GetCorrelationId()
        {
            var correlationIdKey = _configuration.CorrelationIdHeaderKey;
            var correlationId = GetSingleHeaderValue(correlationIdKey);
            correlationId = string.IsNullOrEmpty(correlationId) ? Guid.NewGuid().ToString() : correlationId;
            return correlationId;
        }

        protected string GetTransactionId()
        {
            var transactionIdKey = _configuration.TransactionIdHeaderKey;
            var transactionId = GetSingleHeaderValue(transactionIdKey);
            transactionId = string.IsNullOrEmpty(transactionId) ? Guid.NewGuid().ToString() : transactionId;
            return transactionId;
        }

        protected string GetClusterFromClaims()
        {
            var user = HttpContext.User;
            return user.Claims.FirstOrDefault(claim => claim.Type == _configuration.ClusterClaimsKey)?.Value;
        }

        protected string GetAppFromClaims()
        {
            var user = HttpContext.User;
            return user.Claims.FirstOrDefault(claim => claim.Type == _configuration.AppClaimsKey)?.Value;
        }

        protected string GetPreferredLocation()
        {
            return GetSingleHeaderValue(_configuration.PreferredLocationHeaderKey);
        }

        protected void AddHeaderValue(string headerKey, string headerValue)
        {
            if (Request.Headers.ContainsKey(headerKey))
                Request.Headers[headerKey] = headerValue;
            else
                Request.Headers.Add(headerKey, new Extensions.Primitives.StringValues(headerValue));
        }

        protected string GetSingleHeaderValue(string headerKey)
        {
            if (string.IsNullOrWhiteSpace(headerKey))
                return null;
            var headers = Request.Headers;
            if (headers.ContainsKey(headerKey))
            {
                var header = headers[headerKey];
                return header.FirstOrDefault();
            }
            return null;
        }
    }
}
