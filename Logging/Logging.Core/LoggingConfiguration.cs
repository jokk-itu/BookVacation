using Microsoft.Extensions.Configuration;

namespace Logging;

public class LoggingConfiguration
{
    public string SeqUri { get; }

    public IDictionary<string, string> Overrides { get; }
    
    public LoggingConfiguration(IConfiguration configuration)
    {
        SeqUri = configuration["SeqUri"];
        Overrides = configuration.GetSection("Overrides").Get<IDictionary<string, string>>();
        
        StartupLogger.LogInformation<LoggingConfiguration>("Configuration: SeqUri = {LoggingConfiguration:SeqUri}", SeqUri);
        StartupLogger.LogInformation<LoggingConfiguration>("Configuration: Overrides = {Overrides}", Overrides);
    }
}