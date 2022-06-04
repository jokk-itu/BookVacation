using DocumentClient;
using EventDispatcher;
using FluentValidation;
using HotelService.Infrastructure.CourierActivities;
using HotelService.Infrastructure.Requests;
using HotelService.Infrastructure.Validators;
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
        services.AddMediator(typeof(MediatorRegistration).Assembly);
        services.AddRavenDb(configuration.GetSection("RavenSettings"));
        services.AddValidatorsFromAssembly(typeof(FluentValidatorRegistration).Assembly);
        services.AddEventBus(configuration,
            configurator => { configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>(); });
        return services;
    }
}