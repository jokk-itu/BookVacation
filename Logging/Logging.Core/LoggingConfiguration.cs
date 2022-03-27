using Microsoft.Extensions.Configuration;
using Serilog;

namespace Logging;

public class LoggingConfiguration
{
    public string SeqUri { get; }

    public LoggingConfiguration(IConfiguration configuration)
    {
        var logger = Log.Logger.ForContext<LoggingConfiguration>();
        SeqUri = configuration["SeqUri"];

        logger.Information("Configuration: SeqUri = {LoggingConfiguration:SeqUri}", SeqUri);
    }
}