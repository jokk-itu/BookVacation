using EventBusTransmitting.Filters;
using EventBusTransmitting.Observers;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.PrometheusIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBusTransmitting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration,
        Action<IServiceCollectionBusConfigurator>? callback = null)
    {
        return services.AddMassTransit(configurator =>
        {
            callback?.Invoke(configurator);
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.AddDelayedMessageScheduler();
            configurator.UsingRabbitMq((busContext, factoryConfigurator) =>
            {
                factoryConfigurator.UseConsumeFilter(typeof(LogConsumeFilter<>), busContext);
                factoryConfigurator.UseSendFilter(typeof(LogSendFilter<>), busContext);
                factoryConfigurator.UsePublishFilter(typeof(LogPublishFilter<>), busContext);
                factoryConfigurator.UseExecuteActivityFilter(typeof(LogExecuteFilter<>), busContext);
                factoryConfigurator.UseCompensateActivityFilter(typeof(LogCompensateFilter<>), busContext);

                factoryConfigurator.ConnectBusObserver(
                    new BusObserver(busContext.GetRequiredService<ILogger<BusObserver>>()));
                factoryConfigurator.ConnectSendObserver(
                    new SendObserver(busContext.GetRequiredService<ILogger<SendObserver>>()));
                factoryConfigurator.ConnectPublishObserver(
                    new PublishObserver(busContext.GetRequiredService<ILogger<PublishObserver>>()));
                factoryConfigurator.ConnectConsumeObserver(
                    new ConsumeObserver(busContext.GetRequiredService<ILogger<ConsumeObserver>>()));
                factoryConfigurator.ConnectReceiveObserver(
                    new ReceiveObserver(busContext.GetRequiredService<ILogger<ReceiveObserver>>()));

                factoryConfigurator.UseDelayedMessageScheduler();
                
                factoryConfigurator.UsePrometheusMetrics();

                var hostname = configuration["EventBus:Hostname"];
                var port = configuration["EventBus:Port"];
                factoryConfigurator.Host($"rabbitmq://{hostname}:{port}", hostConfigurator =>
                {
                    hostConfigurator.Username(configuration["EventBus:Username"]);
                    hostConfigurator.Password(configuration["EventBus:Password"]);
                });
                factoryConfigurator.ConfigureEndpoints(busContext);
            });
        });
    }
}