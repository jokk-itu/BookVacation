using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;

namespace Logging.Core;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration ConfigureLogging(this LoggerConfiguration loggerConfiguration)
    {
        var assembly = Assembly.GetCallingAssembly();
        return loggerConfiguration
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
                Matching.FromSource("Mediator").Invoke(logEvent) && logEvent.Level < LogEventLevel.Information);
    }
}