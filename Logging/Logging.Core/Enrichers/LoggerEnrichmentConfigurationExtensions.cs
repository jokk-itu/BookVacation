using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Configuration;

namespace Logging.Enrichers;

public static class LoggerEnrichmentConfigurationExtensions
{
    public static LoggerConfiguration WithRequestId(
        this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration, IServiceProvider serviceProvider)
    {
        return loggerEnrichmentConfiguration.With(serviceProvider.GetRequiredService<RequestIdEnricher>());
    }
    
    public static LoggerConfiguration WithCorrelationId(
        this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration, IServiceProvider serviceProvider)
    {
        return loggerEnrichmentConfiguration.With(serviceProvider.GetRequiredService<CorrelationIdEnricher>());
    }
}