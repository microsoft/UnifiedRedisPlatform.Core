using System;
using System.Net;
using AppInsights.EnterpriseTelemetry;
using Microsoft.AspNetCore.Http;
using AppInsights.EnterpriseTelemetry.Web.Extension.Middlewares;
using Microsoft.UnifiedPlatform.Service.Common.AppExceptions;

namespace Microsoft.UnifiedRedisPlatform.Manager.API.ExceptionHandlers
{
    public class UnauthorizedUserExceptionHandler : IGlobalExceptionHandler
    {
        private readonly ILogger _logger;
        public UnauthorizedUserExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void Handle(Exception exception, HttpContext httpContext, string correlationId, string transactionId)
        {
            if (exception is UnauthorizedUserException)
            {
                _logger.Log(exception);
                if (httpContext.Response.HasStarted)
                    return;

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                httpContext.Response.WriteAsync(exception.Message).Wait();
            }
        }
    }
}
