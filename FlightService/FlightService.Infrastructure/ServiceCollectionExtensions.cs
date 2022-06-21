using System.Reflection;
using DocumentClient;
using EventDispatcher;
using FlightService.Infrastructure.CourierActivities;
using FluentValidation;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediator(Assembly.GetExecutingAssembly());
        services.AddRavenDb(configuration.GetSection("RavenSettings"));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddEventBus(configuration,
            configurator => { configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>(); });
        return services;
    }
}