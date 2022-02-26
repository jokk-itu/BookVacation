using EventDispatcher;
using MassTransit;
using Prometheus.Client.Collectors;
using Prometheus.Client.DependencyInjection;
using Prometheus.Client.MetricServer;
using Prometheus.SystemMetrics;
using Serilog;
using Serilog.Events;
using TrackingService.Infrastructure.Consumers;

namespace TrackingService.Worker;

public static class Program
{
    public static void Main(string[] args)
    {
        var logConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile("appsettings.Development.json", true, true)
            .AddEnvironmentVariables()
            .Build()
            .GetSection("Logging");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Seq(logConfiguration["SeqUri"])
            .CreateBootstrapLogger();

        try
        {
            var builder = CreateHostBuilder(args).Build();
            var metricServer = builder.Services.GetRequiredService<IMetricServer>();
            metricServer.Start();
            builder.Run();
            metricServer.Stop();
        }
        catch (Exception e)
        {
            Log.Error(e, "Unhandled exception during startup");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddEventBus(hostContext.Configuration,
                    configurator => { configurator.AddConsumersFromNamespaceContaining<RoutingSlipEventConsumer>(); });
                services.AddHostedService<EventBusWorker>();

                var isValidPort = int.TryParse(hostContext.Configuration["Prometheus:Port"], out var port);
                if (!isValidPort)
                    throw new ArgumentException();

                services.AddSystemMetrics();
                services.AddMetricFactory();
                services.AddSingleton<IMetricServer>(sp => new MetricServer(
                    new MetricServerOptions
                    {
                        MapPath = "/metrics",
                        Port = port,
                        CollectorRegistryInstance = sp.GetRequiredService<ICollectorRegistry>(),
                        UseDefaultCollectors = true
                    }));
            }).UseSerilog((context, serviceProvider, config) =>
            {
                var seqUri = context.Configuration["Logging:SeqUri"];
                config.WriteTo.Seq(seqUri)
                    .Enrich.FromLogContext()
                    .MinimumLevel.Override("TrackingService", LogEventLevel.Information)
                    .MinimumLevel.Override("EventDispatcher", LogEventLevel.Information)
                    .MinimumLevel.Warning();
            });
    }
}