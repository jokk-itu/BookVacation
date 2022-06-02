using EventDispatcher;
using FluentValidation.AspNetCore;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;
using VacationService.Api;
using VacationService.Api.Validators;

var logConfiguration = new LoggingConfiguration(new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile("appsettings.Development.json", true, true)
    .AddEnvironmentVariables()
    .Build()
    .GetSection("Logging"));

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, serviceProvider, configuration) =>
{
    configuration.ConfigureLogging(logConfiguration);
});

builder.WebHost.ConfigureServices(services =>
{
    services.AddFluentValidation(options =>
    {
        options.DisableDataAnnotationsValidation = true;
        options.AutomaticValidationEnabled = true;
        options.RegisterValidatorsFromAssemblies(new[]
        {
            typeof(FluentValidatorRegistration).Assembly
        });
    });
    services.AddEventBus(builder.Configuration);
    services.AddRouting(options => options.LowercaseUrls = true);
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddApiVersioning(config => { config.ReportApiVersions = true; });
    services.AddVersionedApiExplorer(config =>
    {
        config.GroupNameFormat = "'v'VVV";
        config.SubstituteApiVersionInUrl = true;
    });
    services.AddSwaggerGen();
    services.ConfigureOptions<ConfigureSwaggerOptions>();
    services.AddSystemMetrics();
});

StartupLogger.Run(() =>
{
    var app = builder.Build();
    if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSerilogRequestLogging();
    app.UseHttpMetrics();

    app.MapControllers();
    app.MapMetrics();
    app.MapHealthChecks("health/live", new HealthCheckOptions
    {
        ResultStatusCodes = new Dictionary<HealthStatus, int>
        {
            { HealthStatus.Healthy, StatusCodes.Status200OK }, { HealthStatus.Degraded, StatusCodes.Status200OK },
            { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable }
        },
        AllowCachingResponses = false
    });
    app.MapHealthChecks("health/ready", new HealthCheckOptions
    {
        Predicate = registration => registration.Tags.Contains("ready"),
        ResultStatusCodes = new Dictionary<HealthStatus, int>
        {
            { HealthStatus.Healthy, StatusCodes.Status200OK }, { HealthStatus.Degraded, StatusCodes.Status200OK },
            { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable }
        },
        AllowCachingResponses = false
    });

    app.Run();
}, new LoggerConfiguration().ConfigureLogging(logConfiguration));

namespace VacationService.Api
{
    public class Program
    {
    }
}