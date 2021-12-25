using EventBusTransmitting;
using MassTransit;
using Prometheus.Client.MetricServer;
using PrometheusWorker;
using Serilog;
using Serilog.Events;
using TrackingService.Consumers;

namespace TrackingService;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = CreateHostBuilder(args).Build();
        var metricServer = builder.Services.GetRequiredService<IMetricServer>();
        metricServer.Start();
        builder.Run();
        metricServer.Stop();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddEventBus(hostContext.Configuration,
                    configurator =>
                    {
                        configurator.AddConsumersFromNamespaceContaining<RoutingSlipEventConsumer>();
                    });
                services.AddHostedService<EventBusWorker>();
                services.AddMetricServer(hostContext.Configuration);
            }).UseSerilog((context, serviceProvider, config) =>
            {
                var seqUri = context.Configuration["Logging:SeqUri"];
                config.WriteTo.Seq(seqUri)
                    .Enrich.FromLogContext()
                    .MinimumLevel.Override("TrackingService", LogEventLevel.Information)
                    .MinimumLevel.Override("EventBusTransmitting", LogEventLevel.Information)
                    .MinimumLevel.Warning();
            });
    }
}