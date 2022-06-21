using System.Reflection;
using BlobStorage;
using EventDispatcher;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketService.Infrastructure.CourierActivities;

namespace TicketService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediator(Assembly.GetExecutingAssembly());
        services.AddEventBus(configuration,
            configurator => { configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>(); });
        services.AddBlobStorage();
        services.AddBlobStorageHealthCheck();
        return services;
    }
}