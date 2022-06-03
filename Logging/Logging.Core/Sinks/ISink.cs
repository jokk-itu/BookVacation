using Serilog;

namespace Logging.Sink;

public interface ISink
{
    void Setup(LoggerConfiguration loggerConfiguration, LoggingConfiguration loggingConfiguration);
}