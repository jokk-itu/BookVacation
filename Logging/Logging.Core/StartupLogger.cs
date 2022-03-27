using Serilog;

namespace Logging;

public static class StartupLogger
{
    public static void Run(Action hostRunner, LoggerConfiguration loggerConfiguration)
    {
        Log.Logger = loggerConfiguration.CreateBootstrapLogger();

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