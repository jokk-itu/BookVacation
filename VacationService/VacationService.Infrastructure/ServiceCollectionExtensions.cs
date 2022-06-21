using System.Reflection;
using EventDispatcher;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VacationService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddEventBus(configuration)
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddMediator(Assembly.GetExecutingAssembly());
    }
}