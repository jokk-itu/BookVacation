using Logging.Constants;
using Logging.Meta;
using Microsoft.AspNetCore.Http;

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
        
        if (httpContext.Request.Headers.TryGetValue(Header.RequestId, out var requestId))
            metaContextAccessor.MetaContext.RequestId = requestId;

        await _next(httpContext);
    }
}