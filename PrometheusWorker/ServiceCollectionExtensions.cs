using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Client.Collectors;
using Prometheus.Client.DependencyInjection;
using Prometheus.Client.MetricServer;
using Prometheus.SystemMetrics;

namespace PrometheusWorker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMetricServer(this IServiceCollection services, IConfiguration configuration)
    {
        var isValidPort = int.TryParse(configuration["Prometheus:Port"], out var port);
        if (!isValidPort)
            throw new ArgumentException();
        
        services.AddSystemMetrics();
        services.AddMetricFactory();
        services.AddSingleton<IMetricServer>(sp => new MetricServer(
            new MetricServerOptions
            {
                MapPath = "/metrics",
                Port = port,
                CollectorRegistryInstance = sp.GetRequiredService<ICollectorRegistry>(),
                UseDefaultCollectors = true
            }));
        return services;
    }
}