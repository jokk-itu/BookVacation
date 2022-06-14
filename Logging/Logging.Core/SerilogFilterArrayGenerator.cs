using Serilog.Events;

namespace Logging;

public static class SerilogFilterArrayGenerator
{
    public static string GenerateArrayAboveLevel(LogEventLevel level)
    {
        switch (level)
        {
            case LogEventLevel.Verbose:
                return "['Debug', 'Information', 'Warning', 'Error', 'Fatal']";
            case LogEventLevel.Debug:
                return "['Information', 'Warning', 'Error', 'Fatal']";
            case LogEventLevel.Information:
                return "['Warning', 'Error', 'Fatal']";
            case LogEventLevel.Warning:
                return "['Error', 'Fatal']";
            case LogEventLevel.Error:
                return "['Fatal']";
            case LogEventLevel.Fatal:
                return "[]";
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
    
    public static string GenerateArrayBelowLevel(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose => "[]",
            LogEventLevel.Debug => "['Verbose']",
            LogEventLevel.Information => "['Verbose', 'Debug']",
            LogEventLevel.Warning => "['Verbose', 'Debug', 'Information']",
            LogEventLevel.Error => "['Verbose', 'Debug', 'Information', 'Warning']",
            LogEventLevel.Fatal => "['Verbose', 'Debug', 'Information', 'Warning', 'Error']",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }
}