using System.Diagnostics;
using GreenPipes;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Filters;

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
        var watch = new Stopwatch();
        watch.Start();
        await next.Send(context);
        watch.Stop();
        _logger.LogInformation("Executed {Message} with {MessageId}, took {Elapsed} ms", context.Message.GetType().Name,
            context.MessageId, watch.ElapsedMilliseconds);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("LogExecute");
    }
}