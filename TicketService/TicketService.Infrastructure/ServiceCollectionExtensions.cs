using Mediator;
using Microsoft.Extensions.DependencyInjection;
using TicketService.Infrastructure.Requests;

namespace TicketService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddMediator(typeof(MediatorRegistration).Assembly);
        return services;
    }
}