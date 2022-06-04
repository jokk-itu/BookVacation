using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck;

public class ReadyHealthCheck : IHealthCheck
{
    public static bool IsReady;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        return Task.FromResult(IsReady ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
    }
}