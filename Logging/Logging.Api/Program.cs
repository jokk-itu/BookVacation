using Logging;
using Logging.Configuration;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLoggingServices();

StartupLogger.Run(() =>
{
    builder.Host.UseSerilog((context, serviceProvider, configuration) =>
        configuration.ConfigureAdvancedLogger(new LoggingConfiguration(builder.Configuration.GetSection("Logging")),
            serviceProvider));
    var app = builder.Build();
    app.UseLogging();
    app.MapGet("/", ([FromServices] ILogger<Program> logger) =>
    {
        logger.LogWarning("Warning Log inside '/'");
        logger.LogInformation("Information Log inside '/'");
        logger.LogDebug("Debug Log inside '/'");
        logger.LogTrace("Verbose Log inside '/'");
        return Results.Ok();
    });

    app.MapGet("/health", ([FromServices] ILogger<Program> logger) => Results.Ok());
    app.MapGet("/metrics", ([FromServices] ILogger<Program> logger) => Results.Ok());

    app.Run();
}, new LoggingConfiguration(builder.Configuration.GetSection("Logging")));