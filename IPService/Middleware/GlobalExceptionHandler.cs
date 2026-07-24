using IPService.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IPService.Middleware
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, message) = exception switch
            {
                InvalidIpv4InputException => (400, "Invalid IPv4 address"),
                InvalidCountryCodeException e => (400, e.Message),
                Ip2cException => (502, "Upstream IP lookup service unavailable"),
                IpServiceException e => (400, e.Message),  
                _ => (500, "An unexpected error occurred")
            };

            if (statusCode >= 500)
                _logger.LogError(exception, "Unhandled exception → {StatusCode}", statusCode);
            else
                _logger.LogWarning("Client error → {StatusCode}: {Message}", statusCode, message);

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails { Status = statusCode, Title = message },
                cancellationToken);

            return true;   
        }
    }
}