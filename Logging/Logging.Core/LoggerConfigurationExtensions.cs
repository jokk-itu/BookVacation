using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;

namespace Logging;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration ConfigureStartupLogging(this LoggerConfiguration loggerConfiguration,
        LoggingConfiguration configuration, string serviceName)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));
        
        var assembly = Assembly.GetCallingAssembly();
        var root = assembly.GetName().Name?.Split('.')[0] ?? "NotFound";
        loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithMemoryUsage()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithExceptionDetails()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Assembly", assembly.GetName().Name)
            .Enrich.WithProperty("AssemblyVersion", assembly.GetName().Version)
            .Enrich.WithProperty("Application", serviceName)
            .MinimumLevel.Warning()
            .MinimumLevel.Override("Serilog", LogEventLevel.Information)
            .MinimumLevel.Override("Mediator", LogEventLevel.Information)
            .MinimumLevel.Override("EventDispatcher", LogEventLevel.Information)
            .MinimumLevel.Override("Logging", LogEventLevel.Information)
            .MinimumLevel.Override(root, LogEventLevel.Information)
            .WriteTo.Seq(configuration.SeqUri)
            .WriteTo.Console();

        return loggerConfiguration;
    }
    
    public static LoggerConfiguration ConfigureAdvancedLogging(this LoggerConfiguration loggerConfiguration, LoggingConfiguration configuration, string serviceName)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));
        
        var assembly = Assembly.GetCallingAssembly();
        var root = assembly.GetName().Name?.Split('.')[0] ?? "NotFound";
        Console.WriteLine(root);
        loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithMemoryUsage()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithExceptionDetails()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Assembly", assembly.GetName().Name)
            .Enrich.WithProperty("AssemblyVersion", assembly.GetName().Version)
            .Enrich.WithProperty("Application", serviceName)
            .MinimumLevel.Warning()
            .MinimumLevel.Override("Serilog", LogEventLevel.Information)
            .MinimumLevel.Override("Mediator", LogEventLevel.Information)
            .MinimumLevel.Override("EventDispatcher", LogEventLevel.Information)
            .MinimumLevel.Override("Logging", LogEventLevel.Information)
            .MinimumLevel.Override(root, LogEventLevel.Information)
            .WriteTo.Seq(configuration.SeqUri)
            .WriteTo.Console();

        return loggerConfiguration;
    }
}