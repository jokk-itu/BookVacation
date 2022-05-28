using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;

namespace Logging;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration ConfigureLogging(this LoggerConfiguration loggerConfiguration, LoggingConfiguration configuration)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));
        
        var assembly = Assembly.GetCallingAssembly();
        return loggerConfiguration
            .SetupEnrichers(assembly, configuration.ServiceName)
            .SetupBaseOverrides(assembly, configuration.ServiceName)
            .SetupCustomOverrides(configuration)
            .SetupSeqIfEnabled(configuration)
            .SetupConsoleIfEnabled(configuration)
            .SetupExpressions();
    }

    private static LoggerConfiguration SetupBaseOverrides(this LoggerConfiguration loggerConfiguration, Assembly assembly, string serviceName)
    {
        return loggerConfiguration
            .MinimumLevel.Warning()
            .MinimumLevel.Override(serviceName, LogEventLevel.Information)
            .MinimumLevel.Override("Serilog", LogEventLevel.Information)
            .MinimumLevel.Override("Mediator", LogEventLevel.Information)
            .MinimumLevel.Override("EventDispatcher", LogEventLevel.Information)
            .MinimumLevel.Override("Logging", LogEventLevel.Information)
            .MinimumLevel.Override("DocumentClient", LogEventLevel.Information)
            .MinimumLevel.Override(assembly.GetName().Name?.Split('.')[0] ?? "NotFound", LogEventLevel.Information);
    }

    private static LoggerConfiguration SetupEnrichers(this LoggerConfiguration loggerConfiguration, Assembly assembly, string serviceName)
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

    private static LoggerConfiguration SetupExpressions(this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration
            .Filter.ByExcluding("RequestPath like '/health%' and @l in ['Verbose', 'Debug', 'Information']")
            .Filter.ByExcluding("RequestPath like '/metrics%' and @l in ['Verbose', 'Debug', 'Information']");
    }

    private static LoggerConfiguration SetupCustomOverrides(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration configuration)
    {
        foreach (var pair in configuration.Overrides)
        {
            loggerConfiguration.MinimumLevel.Override(pair.Key, pair.Value);
        }
        return loggerConfiguration;
    }

    private static LoggerConfiguration SetupConsoleIfEnabled(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration configuration)
    {
        if (!configuration.LogToConsole)
            return loggerConfiguration;

        return loggerConfiguration.WriteTo.Console();
    }

    private static LoggerConfiguration SetupSeqIfEnabled(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration configuration)
    {
        if (!configuration.LogToSeq)
            return loggerConfiguration;

        if (!Uri.IsWellFormedUriString(configuration.SeqUri, UriKind.Absolute))
            throw new UriFormatException($"Invalid {nameof(configuration.SeqUri)} set to {configuration.SeqUri}");

        return loggerConfiguration.WriteTo.Seq(configuration.SeqUri);
    }
}