using Logging.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Logging.Middlewares;

public class RequestIdMiddleware
{
    private readonly RequestDelegate _next;

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, ILogger<RequestIdMiddleware> logger)
    {
        var requestIdLogProperty = Guid.NewGuid();
        if (httpContext.Request.Headers.TryGetValue(Header.RequestId, out var requestId) &&
            Guid.TryParse(requestId, out var parsedRequestId))
        {
            requestIdLogProperty = parsedRequestId;
        }

        using (logger.BeginScope(new Dictionary<string, string>()
               {
                   { "RequestId", requestIdLogProperty.ToString() }
               }))
        {
            await _next(httpContext);
        }
    }
}