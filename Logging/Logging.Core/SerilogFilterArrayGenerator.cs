using Serilog.Events;

namespace Logging;

public static class SerilogFilterArrayGenerator
{
    public static string GenerateArrayAboveLevel(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose => "['Verbose', 'Debug', 'Information', 'Warning', 'Error', 'Fatal']",
            LogEventLevel.Debug => "['Debug', 'Information', 'Warning', 'Error', 'Fatal']",
            LogEventLevel.Information => "['Information', 'Warning', 'Error', 'Fatal']",
            LogEventLevel.Warning => "['Warning', 'Error', 'Fatal']",
            LogEventLevel.Error => "['Warning', 'Error', 'Fatal']",
            LogEventLevel.Fatal => "['Fatal']",
        };
    }
    
    public static string GenerateArrayBelowLevel(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose => "['Verbose']",
            LogEventLevel.Debug => "['Verbose', 'Debug']",
            LogEventLevel.Information => "['Verbose', 'Debug', 'Information']",
            LogEventLevel.Warning => "['Verbose', 'Debug', 'Information', 'Warning']",
            LogEventLevel.Error => "['Verbose', 'Debug', 'Information', 'Warning', 'Error']",
            LogEventLevel.Fatal => "['Verbose', 'Debug', 'Information', 'Warning', 'Error', 'Fatal']",
        };
    }
}