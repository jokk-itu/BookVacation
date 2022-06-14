using Logging.Configuration;
using Serilog;

namespace Logging.Sinks;

public interface ISink
{
    void Setup(LoggerConfiguration loggerConfiguration, LoggingConfiguration loggingConfiguration);
}