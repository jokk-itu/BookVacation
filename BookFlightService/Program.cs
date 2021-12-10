using BookFlightService.CourierActivities;
using BookFlightService.StateMachines.BookFlightStateMachine;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4j.Driver;
using Serilog;
using Serilog.Events;

namespace BookFlightService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(configurator =>
                    {
                        configurator.SetKebabCaseEndpointNameFormatter();
                        configurator.AddDelayedMessageScheduler();
                        configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>();
                        configurator.AddSagaStateMachine<BookFlightStateMachine, BookFlightStateMachineInstance>()
                            .MongoDbRepository(mongodbConfigurator =>
                            {
                                mongodbConfigurator.Connection = "mongodb://admin:admin@localhost:27017";
                                mongodbConfigurator.DatabaseName = "bookflights";
                            });
                        configurator.UsingRabbitMq((busContext, factoryConfigurator) =>
                        {
                            factoryConfigurator.UseDelayedMessageScheduler();
                            var hostname = hostContext.Configuration["EventBus:Hostname"];
                            var port = hostContext.Configuration["EventBus:Port"];
                            factoryConfigurator.Host($"rabbitmq://{hostname}:{port}", hostConfigurator =>
                            {
                                hostConfigurator.Username(hostContext.Configuration["EventBus:Username"]);
                                hostConfigurator.Password(hostContext.Configuration["EventBus:Password"]);
                            });
                            factoryConfigurator.ConfigureEndpoints(busContext);
                        });
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
                        .MinimumLevel.Override("BookFlightService", LogEventLevel.Information)
                        .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
                        .MinimumLevel.Warning();
                });
        }
    }
}