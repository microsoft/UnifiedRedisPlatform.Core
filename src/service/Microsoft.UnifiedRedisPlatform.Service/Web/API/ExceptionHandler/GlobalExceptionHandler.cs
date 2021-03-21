using System;
using System.Net;
using AppInsights.EnterpriseTelemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.CQRS.Exceptions;
using AppInsights.EnterpriseTelemetry.Context;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using AppInsights.EnterpriseTelemetry.Web.Extension.Middlewares;
using Microsoft.UnifiedPlatform.Service.Common.AppExceptions;

namespace Microsoft.UnifiedRedisPlatform.Service.API.ExceptionHandler
{
    public class GlobalExceptionHandler : IGlobalExceptionHandler
    {
        private readonly ILogger _logger;
        private readonly string _correlationIdHeader;

        public GlobalExceptionHandler(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _correlationIdHeader = configuration.GetValue<string>("Application:CorrelationIdHeaderKey");
        }

        public void Handle(Exception exception, HttpContext httpContext, string correlationId, string transactionId)
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string reasonPhrase = "OOPS! Somethin went wrong";
            string responseMessage = $"OOPS! Something went wrong. Please contact support with the correlation ID - {GetCorrelationId(httpContext.Request)}";

            if (exception is RequestValidationException)
            {
                var requestValidationException = exception as RequestValidationException;
                statusCode = (int)HttpStatusCode.BadRequest;
                reasonPhrase = requestValidationException.ValidationErrorMessage;
                responseMessage = requestValidationException.Message;
            }
            if (exception is UnauthenticatedAppException ||
                exception is InvalidAppException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                reasonPhrase = exception.Message;
                responseMessage = exception.Message;
            }
            if (exception is InvalidAppException || exception is InvalidClusterException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                reasonPhrase = exception.Message;
                responseMessage = exception.Message;
            }

            var exceptionContext = new ExceptionContext()
            {
                Exception = exception
            };
            _logger.Log(exceptionContext);

            httpContext.Response.Clear();
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = reasonPhrase;
            httpContext.Response.WriteAsync(responseMessage).Wait();
        }

        private string GetCorrelationId(HttpRequest request)
        {
            if (request.Headers.ContainsKey(_correlationIdHeader))
            {
                var correlationId = request.Headers[_correlationIdHeader].ToString();
                correlationId = string.IsNullOrEmpty(correlationId) ? Guid.NewGuid().ToString() : correlationId;
                return correlationId;
            }
            return Guid.NewGuid().ToString();
        }
    }
}
