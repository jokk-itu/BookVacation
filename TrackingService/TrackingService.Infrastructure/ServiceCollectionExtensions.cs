using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Raven.DependencyInjection;
using TrackingService.Infrastructure.Requests;

namespace TrackingService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(MediatorRegistration).Assembly);
        services.AddRavenDbDocStore();
        services.AddRavenDbAsyncSession();
        return services;
    }
}