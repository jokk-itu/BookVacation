using EventDispatcher;
using Logging;
using MassTransit;
using Minio;
using Polly;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;
using TicketService.Api;
using TicketService.Infrastructure;
using TicketService.Infrastructure.CourierActivities;
using TicketService.Infrastructure.Services;
using ConnectionException = Minio.Exceptions.ConnectionException;

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
    configuration.ConfigureAdvancedLogging(logConfiguration, builder.Configuration["ServiceName"]);
});

builder.WebHost.ConfigureServices(services =>
{
    services.AddInfrastructureServices();
    services.AddEventBus(builder.Configuration,
        configurator => { configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>(); });
    services.AddMassTransitHostedService();
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
        var minioClient = new MinioClient(configuration.Uri, configuration.Username, configuration.Password);
        minioClient.WithTimeout(5000);
        minioClient.SetTraceOn(sp.GetRequiredService<MinioLogger>());
        minioClient.WithRetryPolicy(async callback => await Policy
            .Handle<ConnectionException>()
            .WaitAndRetryAsync(3, (retryCount) => TimeSpan.FromSeconds(retryCount * 2))
            .ExecuteAsync(async () => await callback()));
        return minioClient;
    });
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

    app.Run();
}, new LoggerConfiguration().ConfigureStartupLogging(logConfiguration, builder.Configuration["ServiceName"]));

namespace TicketService.Api
{
    public partial class Program
    {
    }
}