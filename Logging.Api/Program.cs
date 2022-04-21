using Logging;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

StartupLogger.Run(() =>
{
    builder.Host.UseSerilog((context, services, configuration) => configuration.ConfigureLogging(new LoggingConfiguration(builder.Configuration.GetSection("Logging"))));
    var app = builder.Build();
    app.UseSerilogRequestLogging();
    app.MapGet("/", ([FromServices] ILogger<Program> logger) =>
    {
        logger.LogInformation("Information Log inside '/'");
        logger.LogDebug("Debug Log inside '/'");
        return Results.Ok();
    });

    app.Run();
}, new LoggerConfiguration().ConfigureLogging(new LoggingConfiguration(builder.Configuration.GetSection("Logging"))));