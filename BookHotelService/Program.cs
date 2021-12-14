
using BookHotelService.CourierActivities;
using EventBusTransmitting;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4j.Driver;
using Serilog;
using Serilog.Events;

namespace BookHotelService
{
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
                    services.AddEventBus(hostContext.Configuration, configurator =>
                    {
                        configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>();
                    });
                    services.AddHostedService<Worker>();
                    services.AddSingleton(_ => GraphDatabase.Driver(
                        "neo4j+s://ba36ce5c.databases.neo4j.io:7687",
                        AuthTokens.Basic(
                            "neo4j",
                            "t-czGssSqfZL_ADeQdMF1nw4_23AhEhMypAUANleSCY")));
                }).UseSerilog((context, serviceProvider, config) =>
                {
                    var seqUri = context.Configuration["Logging:SeqUri"];
                    config.WriteTo.Seq(seqUri)
                        .Enrich.FromLogContext()
                        .MinimumLevel.Override("BookHotelService", LogEventLevel.Information)
                        .MinimumLevel.Warning();
                });
        }
    }
}