using Serilog;

namespace Logging;

public static class StartupLogger
{
    public static void Run(Action hostRunner, LoggingConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration().ConfigureLogging(configuration).CreateBootstrapLogger();

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

    public static void LogVerbose<TSource>(string message, params object?[] args)
    {
        // ReSharper disable once ContextualLoggerProblem
        Log.Logger.ForContext<TSource>().Verbose(message, args);
    }

    public static void LogDebug<TSource>(string message, params object?[] args)
    {
        // ReSharper disable once ContextualLoggerProblem
        Log.Logger.ForContext<TSource>().Debug(message, args);
    }

    public static void LogInformation<TSource>(string message, params object?[] args)
    {
        // ReSharper disable once ContextualLoggerProblem
        Log.Logger.ForContext<TSource>().Information(message, args);
    }

    public static void LogWarning<TSource>(string message, params object?[] args)
    {
        // ReSharper disable once ContextualLoggerProblem
        Log.Logger.ForContext<TSource>().Warning(message, args);
    }

    public static void LogError<TSource>(string message, params object?[] args)
    {
        // ReSharper disable once ContextualLoggerProblem
        Log.Logger.ForContext<TSource>().Error(message, args);
    }

    public static void LogFatal<TSource>(string message, params object?[] args)
    {
        // ReSharper disable once ContextualLoggerProblem
        Log.Logger.ForContext<TSource>().Fatal(message, args);
    }
}