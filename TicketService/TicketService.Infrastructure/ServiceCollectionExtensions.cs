using EventDispatcher;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketService.Infrastructure.CourierActivities;
using TicketService.Infrastructure.Requests;

namespace TicketService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediator(typeof(MediatorRegistration).Assembly);
        services.AddEventBus(configuration,
            configurator => { configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>(); });
        return services;
    }
}