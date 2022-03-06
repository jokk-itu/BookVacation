using CarService.Infrastructure.Requests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Raven.DependencyInjection;

namespace CarService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(AssemblyRegistration).Assembly);
        services.AddRavenDbDocStore();
        services.AddRavenDbAsyncSession();
        return services;
    }
}