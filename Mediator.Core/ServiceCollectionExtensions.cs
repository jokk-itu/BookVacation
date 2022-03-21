using System.Reflection;
using Mediator.PipelineBehaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Assembly requests)
    {
        services.AddMediatR(requests);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
        return services;
    }
}