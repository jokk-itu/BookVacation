using System.Reflection;
using Logging.Sink;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace Logging;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration ConfigureLogging(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration configuration)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        var assembly = Assembly.GetCallingAssembly();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                          Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        if (environment == "Development")
            loggerConfiguration.MinimumLevel.Verbose();
        else
            loggerConfiguration.MinimumLevel.Information();

        loggerConfiguration
            .SetupEnrichers(assembly, configuration.ServiceName)
            .SetupBaseOverrides(assembly, configuration.ServiceName)
            .SetupCustomOverrides(configuration)
            .SetupExpressions();

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

            loggerConfiguration.WriteTo.Logger(lg => ((ISink)sink).Setup(lg, configuration));
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
            .MinimumLevel.Override(assembly.GetName().Name?.Split('.')[0] ?? "NotFound", LogEventLevel.Information);
    }

    private static LoggerConfiguration SetupEnrichers(this LoggerConfiguration loggerConfiguration, Assembly assembly,
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

    private static LoggerConfiguration SetupExpressions(this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration
            .Filter.ByExcluding("RequestPath like '/health%' and @l in ['Verbose', 'Debug', 'Information']")
            .Filter.ByExcluding("RequestPath like '/metrics%' and @l in ['Verbose', 'Debug', 'Information']");
    }

    private static LoggerConfiguration SetupCustomOverrides(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration configuration)
    {
        foreach (var pair in configuration.Overrides) loggerConfiguration.MinimumLevel.Override(pair.Key, pair.Value);

        return loggerConfiguration;
    }
}