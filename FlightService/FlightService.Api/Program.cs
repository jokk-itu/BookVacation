using FlightService.Api;
using FlightService.Api.Validators;
using FlightService.Infrastructure;
using FluentValidation.AspNetCore;
using HealthCheck.Core;
using Logging;
using Logging.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;

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
    configuration.ConfigureAdvancedLogger(logConfiguration, serviceProvider);
});

builder.WebHost.ConfigureServices(services =>
{
    services.AddHealthChecks().AddRavenDB(options =>
    {
        options.Urls = builder.Configuration.GetSection("RavenSettings").GetSection("Urls").Get<string[]>();
    });
    services.AddInfrastructureServices(builder.Configuration);
    services.AddFluentValidation(options =>
    {
        options.DisableDataAnnotationsValidation = true;
        options.AutomaticValidationEnabled = true;
        options.RegisterValidatorsFromAssemblies(new[]
        {
            typeof(FluentValidatorRegistration).Assembly
        });
    });
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
    services.AddLoggingServices();
});

StartupLogger.Run(() =>
{
    var app = builder.Build();
    if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseLogging();
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
    ReadyHealthCheck.IsReady = true;

    app.Run();
}, logConfiguration);