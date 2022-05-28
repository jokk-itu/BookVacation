using CarService.Infrastructure.CourierActivities;
using CarService.Infrastructure.Requests;
using DocumentClient;
using EventDispatcher;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediator(typeof(MediatorRegistration).Assembly);
        services.AddRavenDb(configuration.GetSection("RavenSettings"));
        services.AddEventBus(configuration,
            configurator => { configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>(); });
        return services;
    }
}