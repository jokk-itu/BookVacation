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
        var assembly = Assembly.GetCallingAssembly();
        loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithMemoryUsage()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithExceptionDetails()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", assembly.GetName().Name)
            .MinimumLevel.Warning()
            .Filter.ByExcluding(logEvent =>
                Matching.FromSource(assembly.GetName().Name).Invoke(logEvent) &&
                logEvent.Level < LogEventLevel.Information)
            .Filter.ByExcluding(logEvent =>
                Matching.FromSource("EventDispatcher").Invoke(logEvent) && logEvent.Level < LogEventLevel.Information)
            .Filter.ByExcluding(logEvent =>
                Matching.FromSource("Mediator").Invoke(logEvent) && logEvent.Level < LogEventLevel.Information)
            .WriteTo.Seq(configuration.SeqUri)
            .WriteTo.Console();

        foreach (var pair in configuration.Overrides)
        {
            var logEvent = Enum.Parse<LogEventLevel>(pair.Key);
            loggerConfiguration.Filter.ByExcluding(x =>
                Matching.FromSource(pair.Value).Invoke(x) && x.Level < logEvent);
        }

        return loggerConfiguration;
    }
}