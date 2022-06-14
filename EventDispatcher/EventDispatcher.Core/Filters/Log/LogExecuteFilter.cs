using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Filters.Log;

public class LogExecuteFilter<T> : IFilter<ExecuteContext<T>> where T : class
{
    private readonly ILogger<LogExecuteFilter<T>> _logger;

    public LogExecuteFilter(ILogger<LogExecuteFilter<T>> logger)
    {
        _logger = logger;
    }

    public async Task Send(ExecuteContext<T> context, IPipe<ExecuteContext<T>> next)
    {
        _logger.LogDebug("Executing message");
        var watch = Stopwatch.StartNew();
        await next.Send(context);
        watch.Stop();
        
        var scope = new Dictionary<string, object>();
        scope.Add("TrackingNumber", context.TrackingNumber);
        scope.Add("ActivityName", context.ActivityName);
        if(context.MessageId is not null)
            scope.Add("MessageId", context.MessageId);
        
        using (_logger.BeginScope(scope))
        {
            _logger.LogInformation("Executed {Message}, took {Elapsed} ms",
                context.Message.GetType().Name, watch.ElapsedMilliseconds);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("ExecuteScope");
    }
}