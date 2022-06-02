using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReadyHealthCheck(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck<ReadyHealthCheck>(
                "Ready",
                HealthStatus.Unhealthy,
                new[] { "ready" });
        return services;
    }
}