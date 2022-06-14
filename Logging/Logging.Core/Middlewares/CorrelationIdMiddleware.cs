using Logging.Constants;
using Meta;
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

        if (httpContext.Request.Headers.TryGetValue(Header.CorrelationId, out var correlationId) &&
            Guid.TryParse(correlationId, out var parsedCorrelationId))
        {
            metaContextAccessor.MetaContext.CorrelationId = parsedCorrelationId;
        }

        await _next(httpContext);
    }
}