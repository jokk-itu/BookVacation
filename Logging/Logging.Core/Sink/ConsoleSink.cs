using Serilog;

namespace Logging.Sink;

public class ConsoleSink : ISink
{
    public void Setup(LoggerConfiguration loggerConfiguration, LoggingConfiguration loggingConfiguration)
    {
        if (loggingConfiguration.LogToConsole)
            loggerConfiguration
                .Enrich.WithProperty("ConsoleProperty", "Console")
                .WriteTo.Console(loggingConfiguration.ConsoleMinimumLogLevel,
                    "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}");
    }
}