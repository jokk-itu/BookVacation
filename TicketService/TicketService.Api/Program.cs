using HealthCheck.Core;
using Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Minio;
using Minio.Exceptions;
using Polly;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;
using TicketService.Api;
using TicketService.Infrastructure;
using TicketService.Infrastructure.Services;

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
    services.AddInfrastructureServices(builder.Configuration);
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

    services.AddTransient(sp =>
        new MinioConfiguration(sp.GetRequiredService<IConfiguration>().GetSection("Minio")));
    services.AddTransient(sp => new MinioLogger(sp.GetRequiredService<ILogger<MinioLogger>>()));
    services.AddTransient<IMinioService, MinioService>();
    services.AddSingleton(sp =>
    {
        var configuration = sp.GetRequiredService<MinioConfiguration>();
        var minioClient = new MinioClient()
            .WithEndpoint(configuration.Uri)
            .WithCredentials(configuration.Username, configuration.Password)
            .Build();
        minioClient.WithTimeout(5000);
        minioClient.SetTraceOn(sp.GetRequiredService<MinioLogger>());
        minioClient.WithRetryPolicy(async callback => await Policy
            .Handle<ConnectionException>()
            .WaitAndRetryAsync(3, retryCount => TimeSpan.FromSeconds(retryCount * 2))
            .ExecuteAsync(async () => await callback()));
        return minioClient;
    });
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