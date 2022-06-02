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
    
    public LogEventLevel SeqMinimumLogLevel { get; }
    public LogEventLevel ConsoleMinimumLogLevel { get; }

    public LoggingConfiguration(IConfiguration configuration)
    {
        var logger = Log.ForContext<LoggingConfiguration>();
        SeqUri = configuration["SeqUri"];
        LogToSeq = configuration.GetValue("LogToSeq", false);
        LogToConsole = configuration.GetValue("LogToConsole", false);
        Overrides = configuration.GetSection("Overrides").Get<Dictionary<string, string>>()?.Aggregate(
            new Dictionary<string, LogEventLevel>(),
            (acc, pair) =>
            {
                if (!Enum.TryParse(pair.Value, out LogEventLevel logEventLevel))
                    throw new LoggingConfigurationException(typeof(LogEventLevel), pair.Value);
                
                acc.Add(pair.Key, logEventLevel);
                return acc;
            }) ?? new Dictionary<string, LogEventLevel>();
        ServiceName = configuration["ServiceName"];
        SeqMinimumLogLevel = GetLogLevel(configuration["SeqMinimumLogLevel"]);
        ConsoleMinimumLogLevel = GetLogLevel(configuration["ConsoleMinimumLoglevel"]);
       
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(SeqUri), SeqUri);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(LogToSeq), LogToSeq);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(LogToConsole),  LogToConsole);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(Overrides), JsonSerializer.Serialize(Overrides, new JsonSerializerOptions {WriteIndented = true}));
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(ServiceName), ServiceName);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(SeqMinimumLogLevel), SeqMinimumLogLevel);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(ConsoleMinimumLogLevel), ConsoleMinimumLogLevel);
    }

    private LogEventLevel GetLogLevel(string value)
    {
        if (Enum.TryParse(value, out LogEventLevel logEventLevel))
        {
            return logEventLevel;
        }
        return LogEventLevel.Information;
    }
}