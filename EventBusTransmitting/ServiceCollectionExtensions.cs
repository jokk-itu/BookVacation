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
                factoryConfigurator.UsePrometheusMetrics(options => { }, configuration["ServiceName"]);

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

                var hostname = configuration["EventBus:Hostname"] ?? throw new ArgumentNullException();
                var port = configuration["EventBus:Port"] ?? throw new ArgumentNullException();
                factoryConfigurator.Host($"rabbitmq://{hostname}:{port}", hostConfigurator =>
                {
                    hostConfigurator.Username(configuration["EventBus:Username"] ?? throw new ArgumentNullException());
                    hostConfigurator.Password(configuration["EventBus:Password"] ?? throw new ArgumentNullException());
                });
                factoryConfigurator.ConfigureEndpoints(busContext);
            });
        });
    }
}