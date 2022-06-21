using Logging.Configuration;
using Serilog;
using Serilog.Filters;

namespace Logging.Sinks;

public class ConsoleSink : ISink
{
    public void Setup(LoggerConfiguration loggerConfiguration, LoggingConfiguration loggingConfiguration)
    {
        if (!loggingConfiguration.LogToConsole)
            return;

        loggerConfiguration
            .Enrich.WithProperty("ConsoleProperty", "Console")
            .WriteTo.Console(loggingConfiguration.ConsoleMinimumLogLevel,
                "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}");

        foreach (var pair in loggingConfiguration.ConsoleOverrides)
            loggerConfiguration.Filter.ByExcluding(logEvent =>
                Matching.FromSource(pair.Key).Invoke(logEvent) && logEvent.Level <= pair.Value);
    }
}