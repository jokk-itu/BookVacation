using EventBusTransmitting;
using MassTransit;
using Neo4j.Driver;
using PrometheusWorker;
using RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;
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
                        configurator.AddSagaStateMachine<RoutingSlipStateMachine, RoutingSlipStateMachineInstance>()
                            .MongoDbRepository(mongodbConfigurator =>
                            {
                                mongodbConfigurator.Connection = hostContext.Configuration["Mongo:Uri"];
                                mongodbConfigurator.DatabaseName = hostContext.Configuration["Mongo:Database"];
                            });
                    });
                services.AddSingleton(_ => GraphDatabase.Driver(
                    hostContext.Configuration["Neo4j:Uri"],
                    AuthTokens.Basic(
                        hostContext.Configuration["Neo4j:Username"],
                        hostContext.Configuration["Neo4j:Password"])));
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