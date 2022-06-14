using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Logging;

public class LoggingConfiguration
{
    public LoggingConfiguration(IConfiguration configuration)
    {
        var logger = Log.ForContext<LoggingConfiguration>();
        SeqUri = configuration["SeqUri"];
        LogToSeq = configuration.GetValue("LogToSeq", false);
        LogToConsole = configuration.GetValue("LogToConsole", false);
        GlobalOverrides = GetOverrides(configuration, "GlobalOverrides");
        ConsoleOverrides = GetOverrides(configuration, "ConsoleOverrides");
        SeqOverrides = GetOverrides(configuration, "SeqOverrides");
        ServiceName = configuration["ServiceName"];
        SeqMinimumLogLevel = GetLogLevel(configuration["SeqMinimumLogLevel"]);
        ConsoleMinimumLogLevel = GetLogLevel(configuration["ConsoleMinimumLoglevel"]);
        PodName = configuration.GetSection("Pod")?.GetValue<string>("Name") ?? string.Empty;
        NodeName = configuration.GetSection("Node")?.GetValue<string>("Name") ?? string.Empty;

        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(SeqUri), SeqUri);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(LogToSeq), LogToSeq);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(LogToConsole),
            LogToConsole);
        
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(GlobalOverrides),
            JsonSerializer.Serialize(GlobalOverrides, new JsonSerializerOptions { WriteIndented = true }));
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(ConsoleOverrides),
            JsonSerializer.Serialize(ConsoleOverrides, new JsonSerializerOptions { WriteIndented = true }));
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(SeqOverrides),
            JsonSerializer.Serialize(SeqOverrides, new JsonSerializerOptions { WriteIndented = true }));
        
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(ServiceName),
            ServiceName);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(SeqMinimumLogLevel),
            SeqMinimumLogLevel);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(ConsoleMinimumLogLevel),
            ConsoleMinimumLogLevel);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(PodName), PodName);
        logger.Information("Configuration: {ConfigurationKey} = {ConfigurationValue}", nameof(NodeName), NodeName);
    }

    private IDictionary<string, LogEventLevel> GetOverrides(IConfiguration configuration, string key)
    {
        return configuration.GetSection(key).Get<Dictionary<string, string>>()?.Aggregate(
            new Dictionary<string, LogEventLevel>(),
            (acc, pair) =>
            {
                if (!Enum.TryParse(pair.Value, out LogEventLevel logEventLevel))
                    throw new LoggingConfigurationException(typeof(LogEventLevel), pair.Value);

                acc.Add(pair.Key, logEventLevel);
                return acc;
            }) ?? new Dictionary<string, LogEventLevel>();
    }

    public string SeqUri { get; }
    public bool LogToSeq { get; }
    public bool LogToConsole { get; }
    public string ServiceName { get; }
    
    public IDictionary<string, LogEventLevel> GlobalOverrides { get; }
    public IDictionary<string, LogEventLevel> ConsoleOverrides { get; }
    public IDictionary<string, LogEventLevel> SeqOverrides { get; }

    public LogEventLevel SeqMinimumLogLevel { get; }
    public LogEventLevel ConsoleMinimumLogLevel { get; }
    
    public string PodName { get; }
    public string NodeName { get; }

    private LogEventLevel GetLogLevel(string value)
    {
        return Enum.TryParse(value, out LogEventLevel logEventLevel) ? logEventLevel : LogEventLevel.Information;
    }
}