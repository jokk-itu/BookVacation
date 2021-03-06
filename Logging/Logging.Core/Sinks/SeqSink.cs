using Logging.Configuration;
using Serilog;
using Serilog.Filters;

namespace Logging.Sinks;

public class SeqSink : ISink
{
    public void Setup(LoggerConfiguration loggerConfiguration, LoggingConfiguration loggingConfiguration)
    {
        if (!loggingConfiguration.LogToSeq)
            return;

        if (!Uri.IsWellFormedUriString(loggingConfiguration.SeqUri, UriKind.Absolute))
            throw new UriFormatException(
                $"Invalid {nameof(loggingConfiguration.SeqUri)} set to {loggingConfiguration.SeqUri}");

        loggerConfiguration
            .WriteTo.Seq(loggingConfiguration.SeqUri,
                loggingConfiguration.SeqMinimumLogLevel);

        foreach (var pair in loggingConfiguration.SeqOverrides)
            loggerConfiguration.Filter.ByExcluding(logEvent =>
                Matching.FromSource(pair.Key).Invoke(logEvent) && logEvent.Level <= pair.Value);
    }
}