using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Logging;

public class LoggingConfiguration
{
    public string SeqUri { get; }
    public bool LogToSeq { get; }
    public bool LogToConsole { get; }

    public IDictionary<string, LogEventLevel> Overrides { get; } 

    public LoggingConfiguration(IConfiguration configuration)
    {
        var logger = Log.Logger.ForContext<LoggingConfiguration>();
        SeqUri = configuration["SeqUri"];
        LogToSeq = configuration.GetValue<bool>("LogToSeq");
        LogToConsole = configuration.GetValue<bool>("LogToConsole");
        Overrides = configuration.GetSection("Overrides").Get<Dictionary<string, string>>().Aggregate(
            new Dictionary<string, LogEventLevel>(),
            (acc, pair) =>
            {
                if(Enum.TryParse(pair.Value, out LogEventLevel logEventLevel))
                   acc.Add(pair.Key, logEventLevel);
                   
                throw new LoggingConfigurationException(typeof(LogEventLevel), pair.Value);
            });

        logger.Information("Configuration: SeqUri = {LoggingConfiguration:SeqUri}", SeqUri);
        logger.Information("Configuration: LogToSeq = {LoggingConfiguration:LogToSeq}", LogToSeq);
        logger.Information("Configuration: LogToConsole = {LoggingConfiguration:LogToConsole}", LogToConsole);
        logger.Information("Configuration: Overrides = {LoggingConfiguration:Overrides}", JsonSerializer.Serialize(Overrides, new JsonSerializerOptions {WriteIndented = true}));
    }
}