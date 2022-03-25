using Microsoft.Extensions.Configuration;

namespace Logging;

public class LoggingConfiguration
{
    public string SeqUri { get; }

    public LoggingConfiguration(IConfiguration configuration)
    {
        SeqUri = configuration["SeqUri"];

        StartupLogger.LogInformation<LoggingConfiguration>("Configuration: SeqUri = {LoggingConfiguration:SeqUri}", SeqUri);
    }
}