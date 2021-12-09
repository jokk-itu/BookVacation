using System;
using Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RentCarService.Consumers;
using RentCarService.CourierActivities;
using Serilog;
using Serilog.Events;

namespace RentCarService
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
                        configurator.AddRequestClient<RentCar>(new Uri("queue:rent-car"));
                        configurator.AddConsumersFromNamespaceContaining<ConsumerRegistration>();
                        configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>();
                        configurator.UsingRabbitMq((busContext, factoryConfigurator) =>
                        {
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
                }).UseSerilog((context, serviceProvider, config) =>
                {
                    var seqUri = context.Configuration["Logging:SeqUri"];
                    config.WriteTo.Seq(seqUri)
                        .Enrich.FromLogContext()
                        .MinimumLevel.Override("RentCarService", LogEventLevel.Information)
                        .MinimumLevel.Warning();
                });
        }
    }
}