using Logging.Middlewares;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Logging;

public static class WebApplicationExtensions
{
    public static WebApplication UseLogging(this WebApplication app)
    {
        app.UseMiddleware<RequestIdMiddleware>();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseSerilogRequestLogging();
        return app;
    }
}