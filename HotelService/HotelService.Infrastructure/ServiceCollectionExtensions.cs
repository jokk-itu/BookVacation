using System.Reflection;
using DocumentClient;
using EventDispatcher;
using FluentValidation;
using HotelService.Infrastructure.CourierActivities;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelService.Infrastructure;

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