using EventDispatcher;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VacationService.Infrastructure.Requests;
using VacationService.Infrastructure.Validators;

namespace VacationService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddEventBus(configuration)
            .AddValidatorsFromAssembly(typeof(FluentValidatorRegistration).Assembly)
            .AddMediator(typeof(MediatorRegistration).Assembly);
    }
}