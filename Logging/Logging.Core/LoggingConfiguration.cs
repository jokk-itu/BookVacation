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
    public string ServiceName { get; }
    public IDictionary<string, LogEventLevel> Overrides { get; } 

    public LoggingConfiguration(IConfiguration configuration)
    {
        var logger = Log.ForContext<LoggingConfiguration>();
        SeqUri = configuration["SeqUri"];
        LogToSeq = configuration.GetValue("LogToSeq", false);
        LogToConsole = configuration.GetValue("LogToConsole", false);
        Overrides = configuration.GetSection("Overrides").Get<Dictionary<string, string>>().Aggregate(
            new Dictionary<string, LogEventLevel>(),
            (acc, pair) =>
            {
                if (!Enum.TryParse(pair.Value, out LogEventLevel logEventLevel))
                    throw new LoggingConfigurationException(typeof(LogEventLevel), pair.Value);
                
                acc.Add(pair.Key, logEventLevel);
                return acc;
            });
        ServiceName = configuration["ServiceName"];

        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", SeqUri.GetType().Name, SeqUri);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", LogToSeq.GetType().Name, LogToSeq);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", LogToConsole.GetType().Name,  LogToConsole);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", Overrides.GetType().Name, JsonSerializer.Serialize(Overrides, new JsonSerializerOptions {WriteIndented = true}));
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", ServiceName.GetType().Name, ServiceName);
    }
}