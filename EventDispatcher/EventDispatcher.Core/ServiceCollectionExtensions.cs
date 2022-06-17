using EventDispatcher.Filters;
using EventDispatcher.Filters.Log;
using EventDispatcher.Filters.Meta;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventDispatcher;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? callback = null)
    {
        return services.AddMassTransit(configurator =>
        {
            callback?.Invoke(configurator);
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.UsingRabbitMq((busContext, factoryConfigurator) =>
            {
                factoryConfigurator.UseMessageRetry(retryConfigurator =>
                {
                    retryConfigurator.Exponential(10, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(2));
                });
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

                factoryConfigurator.UseConsumeFilter(typeof(MetaConsumerFilter<>), busContext);
                factoryConfigurator.UseSendFilter(typeof(MetaSendFilter<>), busContext);
                factoryConfigurator.UsePublishFilter(typeof(MetaPublishFilter<>), busContext);
                factoryConfigurator.UseCompensateActivityFilter(typeof(MetaCompensateFilter<>), busContext);
                factoryConfigurator.UseExecuteActivityFilter(typeof(MetaExecuteFilter<>), busContext);
                
                factoryConfigurator.UseConsumeFilter(typeof(LogConsumeFilter<>), busContext);
                factoryConfigurator.UseSendFilter(typeof(LogSendFilter<>), busContext);
                factoryConfigurator.UsePublishFilter(typeof(LogPublishFilter<>), busContext);
                factoryConfigurator.UseExecuteActivityFilter(typeof(LogExecuteFilter<>), busContext);
                factoryConfigurator.UseCompensateActivityFilter(typeof(LogCompensateFilter<>), busContext);

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