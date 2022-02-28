using EventDispatcher.Filters;
using EventDispatcher.Observers;
using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.PrometheusIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventDispatcher;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration,
        Action<IServiceCollectionBusConfigurator>? callback = null)
    {
        return services.AddMassTransit(configurator =>
        {
            callback?.Invoke(configurator);
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.UsingRabbitMq((busContext, factoryConfigurator) =>
            {
                factoryConfigurator.UseCircuitBreaker(circuitBreakerConfigurator =>
                {
                    circuitBreakerConfigurator.ActiveThreshold = 2;
                    circuitBreakerConfigurator.ResetInterval = TimeSpan.FromSeconds(5);
                    circuitBreakerConfigurator.TrackingPeriod = TimeSpan.FromSeconds(1);
                    circuitBreakerConfigurator.TripThreshold = 10;
                });
                
                factoryConfigurator.UseKillSwitch(killSwitchOptions => killSwitchOptions
                    .SetActivationThreshold(10)
                    .SetTripThreshold(0.15)
                    .SetRestartTimeout(m: 1));

                factoryConfigurator.UsePrometheusMetrics(serviceName: configuration["ServiceName"]);

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