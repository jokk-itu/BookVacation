using DocumentClient;
using EventDispatcher;
using FluentValidation;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrackingService.Infrastructure.Consumers;
using TrackingService.Infrastructure.Requests;
using TrackingService.Infrastructure.Validators;

namespace TrackingService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediator(typeof(MediatorRegistration).Assembly);
        services.AddRavenDb(configuration.GetSection("RavenSettings"));
        services.AddValidatorsFromAssembly(typeof(FluentValidatorRegistration).Assembly);
        services.AddEventBus(configuration,
            configurator => { configurator.AddConsumersFromNamespaceContaining<ConsumerRegistration>(); });
        return services;
    }
}