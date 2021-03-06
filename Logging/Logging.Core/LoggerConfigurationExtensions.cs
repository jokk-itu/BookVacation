using System.Reflection;
using Logging.Configuration;
using Logging.Enrichers;
using Logging.Sinks;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace Logging;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration ConfigureStartupLogger(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration configuration)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        return loggerConfiguration.ConfigureLogging(configuration);
    }

    public static LoggerConfiguration ConfigureAdvancedLogger(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration configuration, IServiceProvider serviceProvider)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        if (serviceProvider is null)
            throw new ArgumentNullException(nameof(serviceProvider));

        return loggerConfiguration.ConfigureLogging(configuration).SetupAdvancedEnrichers(serviceProvider);
    }

    private static LoggerConfiguration ConfigureLogging(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration configuration)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        var assembly = Assembly.GetEntryAssembly()!;
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                          Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        if (environment == "Development")
            loggerConfiguration.MinimumLevel.Verbose();
        else
            loggerConfiguration.MinimumLevel.Information();

        loggerConfiguration
            .SetupBaseEnrichers(assembly, configuration.ServiceName)
            .SetupKubernetesInformation(configuration)
            .SetupBaseOverrides(assembly, configuration.ServiceName)
            .SetupCustomGlobalOverrides(configuration)
            .SetupExpressions()
            .SetupSinks(configuration);

        return loggerConfiguration;
    }

    private static LoggerConfiguration SetupSinks(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration loggingConfiguration)
    {
        var sinkTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(ISink)));

        foreach (var sinkType in sinkTypes)
        {
            var sink = Activator.CreateInstance(sinkType);
            if (sink is null)
            {
                Log.Error("Sink {Type} needs an empty constructor", sinkType.Name);
                continue;
            }

            loggerConfiguration.WriteTo.Logger(lg => ((ISink)sink).Setup(lg, loggingConfiguration));
        }

        return loggerConfiguration;
    }

    private static LoggerConfiguration SetupBaseOverrides(this LoggerConfiguration loggerConfiguration,
        Assembly assembly, string serviceName)
    {
        return loggerConfiguration
            .MinimumLevel.Override(serviceName, LogEventLevel.Information)
            .MinimumLevel.Override("Serilog.AspnetCore.RequestLoggingMiddleware", LogEventLevel.Information)
            .MinimumLevel.Override("Mediator", LogEventLevel.Information)
            .MinimumLevel.Override("EventDispatcher", LogEventLevel.Information)
            .MinimumLevel.Override("Logging", LogEventLevel.Information)
            .MinimumLevel.Override("DocumentClient", LogEventLevel.Information)
            .MinimumLevel.Override("HealthCheck", LogEventLevel.Information)
            .MinimumLevel.Override(assembly.GetName().Name?.Split('.')[0], LogEventLevel.Information);
    }

    private static LoggerConfiguration SetupBaseEnrichers(this LoggerConfiguration loggerConfiguration,
        Assembly assembly,
        string serviceName)
    {
        return loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithMemoryUsage()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithExceptionDetails()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Assembly", assembly.GetName().Name)
            .Enrich.WithProperty("AssemblyVersion", assembly.GetName().Version)
            .Enrich.WithProperty("Application", serviceName);
    }

    private static LoggerConfiguration SetupAdvancedEnrichers(this LoggerConfiguration loggerConfiguration,
        IServiceProvider serviceProvider)
    {
        return loggerConfiguration
            .Enrich.WithCorrelationId(serviceProvider)
            .Enrich.WithRequestId(serviceProvider);
    }

    private static LoggerConfiguration SetupExpressions(this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration
            .Filter.ByExcluding("RequestPath like '/health%' and @l in ['Verbose', 'Debug', 'Information']")
            .Filter.ByExcluding("RequestPath like '/metrics%' and @l in ['Verbose', 'Debug', 'Information']");
    }

    private static LoggerConfiguration SetupCustomGlobalOverrides(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration configuration)
    {
        foreach (var pair in configuration.GlobalOverrides)
            loggerConfiguration.MinimumLevel.Override(pair.Key, pair.Value);

        return loggerConfiguration;
    }

    private static LoggerConfiguration SetupKubernetesInformation(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration loggingConfiguration)
    {
        if (!string.IsNullOrWhiteSpace(loggingConfiguration.PodName))
            loggerConfiguration.Enrich.WithProperty("PodName", loggingConfiguration.PodName);

        if (!string.IsNullOrWhiteSpace(loggingConfiguration.NodeName))
            loggerConfiguration.Enrich.WithProperty("NodeName", loggingConfiguration.NodeName);

        return loggerConfiguration;
    }
}