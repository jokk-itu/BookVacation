using Serilog;

namespace Logging.Sink;

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
                restrictedToMinimumLevel: loggingConfiguration.SeqMinimumLogLevel);
        
        foreach (var pair in loggingConfiguration.SeqOverrides)
        {
            loggerConfiguration.Filter.ByExcluding(
                $"SourceContext like {pair.Key} and @l in ${SerilogFilterArrayGenerator.GenerateArrayBelowLevel(pair.Value)}");
        }
    }
}