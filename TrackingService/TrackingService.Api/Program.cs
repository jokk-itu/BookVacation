using EventDispatcher;
using FluentValidation.AspNetCore;
using Logging;
using MassTransit;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;
using TrackingService.Api;
using TrackingService.Api.Validators;
using TrackingService.Infrastructure;
using TrackingService.Infrastructure.Consumers;

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
    services.AddHealthChecks().AddRavenDB(options =>
    {
        options.Database = builder.Configuration.GetSection("RavenSettings")["Database"];
        options.Urls = builder.Configuration.GetSection("Urls").Get<string[]>();
    });
    services.AddInfrastructureServices(builder.Configuration);
    services.AddFluentValidation(options =>
    {
        options.DisableDataAnnotationsValidation = true;
        options.AutomaticValidationEnabled = true;
        options.RegisterValidatorsFromAssemblies(new[]
        {
            typeof(FluentValidatorRegistration).Assembly,
            typeof(TrackingService.Infrastructure.Validators.FluentValidatorRegistration).Assembly
        });
    });
    services.AddEventBus(builder.Configuration,
        configurator => { configurator.AddConsumersFromNamespaceContaining<ConsumerRegistration>();});
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

namespace TrackingService.Api
{
    public partial class Program
    {
    }
}