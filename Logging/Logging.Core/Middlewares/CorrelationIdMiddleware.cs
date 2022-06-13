using Logging.Constants;
using Logging.Meta;
using Microsoft.AspNetCore.Http;

namespace Logging.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, IMetaContextAccessor metaContextAccessor)
    {
        metaContextAccessor.MetaContext ??= new MetaContext();
            
        if (httpContext.Request.Headers.TryGetValue(Header.CorrelationId, out var correlationId))
            metaContextAccessor.MetaContext.CorrelationId = correlationId;

        await _next(httpContext);
    }
}