using EventBusTransmitting;
using MassTransit;
using PrometheusWorker;
using RoutingSlipMonitorService.Consumers;
using Serilog;
using Serilog.Events;

namespace RoutingSlipMonitorService;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
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
                    .MinimumLevel.Override("RoutingSlipMonitorService", LogEventLevel.Information)
                    .MinimumLevel.Override("EventBusTransmitting", LogEventLevel.Information)
                    .MinimumLevel.Warning();
            });
    }
}