using HotelService.Infrastructure.Requests;
using Mediator;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Migration.Core;

namespace HotelService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediator(typeof(MediatorRegistration).Assembly);
        services.AddRavenDb(configuration.GetSection("RavenSettings"));
        return services;
    }
}