using System;
using BookHotelService.Consumers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BookHotelService
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
                        //configurator.AddMessageScheduler(new Uri("queue:book-hotel-scheduler"));
                        configurator.SetKebabCaseEndpointNameFormatter();
                        configurator.AddConsumersFromNamespaceContaining<ConsumerRegistration>();
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
                    config.WriteTo.Seq("http://localhost:5341")
                        .Enrich.FromLogContext()
                        .MinimumLevel.Override("BookHotelService", LogEventLevel.Information)
                        .MinimumLevel.Warning();
                });
        }
    }
}