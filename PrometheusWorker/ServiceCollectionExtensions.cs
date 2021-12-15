using Microsoft.Extensions.DependencyInjection;
using Prometheus.Client.Collectors;
using Prometheus.Client.DependencyInjection;
using Prometheus.Client.MetricServer;
using Prometheus.SystemMetrics;

namespace PrometheusWorker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMetricServer(this IServiceCollection services)
    {
        services.AddSystemMetrics();
        services.AddMetricFactory();
        services.AddSingleton<IMetricServer>(sp => new MetricServer(
            new MetricServerOptions
            {
                CollectorRegistryInstance = sp.GetRequiredService<ICollectorRegistry>(),
                UseDefaultCollectors = true
            }));
        services.AddHostedService<MetricServerWorker>();
        return services;
    }
}