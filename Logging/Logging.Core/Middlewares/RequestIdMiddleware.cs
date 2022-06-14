using Logging.Constants;
using Meta;
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

    public async Task Invoke(HttpContext httpContext, IMetaContextAccessor metaContextAccessor)
    {
        metaContextAccessor.MetaContext ??= new MetaContext();
        if (httpContext.Request.Headers.TryGetValue(Header.RequestId, out var requestId) &&
            Guid.TryParse(requestId, out var parsedRequestId))
        {
            metaContextAccessor.MetaContext.RequestId = parsedRequestId;
        }
        
        await _next(httpContext);
    }
}