using System;
using BookVacationService.Consumers;
using BookVacationService.CourierActivities;
using Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BookVacationService
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
                        configurator.AddRequestClient<BookFlight>(new Uri("queue:book-flight"));
                        configurator.AddRequestClient<BookHotel>(new Uri("queue:book-hotel"));
                        configurator.AddRequestClient<RentCar>(new Uri("queue:rent-car"));
                        configurator.AddConsumersFromNamespaceContaining<ConsumerRegistration>();
                        configurator.AddActivitiesFromNamespaceContaining<CourierActivityRegistration>();
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
                        .MinimumLevel.Override("BookVacationService", LogEventLevel.Information)
                        .MinimumLevel.Warning();
                });
        }
    }
}