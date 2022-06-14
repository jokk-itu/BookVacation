using Logging.Configuration;
using Serilog;

namespace Logging;

public static class StartupLogger
{
    public static void Run(Action hostRunner, LoggingConfiguration loggingConfiguration)
    {
        Log.Logger = new LoggerConfiguration().ConfigureStartupLogger(loggingConfiguration).CreateBootstrapLogger();

        try
        {
            hostRunner.Invoke();
        }
        catch (Exception e)
        {
            Log.Logger.Fatal(e, "Unhandled exception occured");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}