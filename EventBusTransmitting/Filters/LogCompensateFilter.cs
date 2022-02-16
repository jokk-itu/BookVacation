using System.Diagnostics;
using GreenPipes;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace EventBusTransmitting.Filters;

public class LogCompensateFilter<T> : IFilter<CompensateContext<T>> where T : class
{
    private readonly ILogger<LogCompensateFilter<T>> _logger;

    public LogCompensateFilter(ILogger<LogCompensateFilter<T>> logger)
    {
        _logger = logger;
    }

    public async Task Send(CompensateContext<T> context, IPipe<CompensateContext<T>> next)
    {
        _logger.LogDebug("Compensating message");
        var watch = new Stopwatch();
        watch.Start();
        await next.Send(context);
        watch.Stop();
        _logger.LogInformation("Compensated {Message} with {MessageId}, took {Elapsed} ms",
            context.Message.GetType().Name, context.MessageId, watch.ElapsedMilliseconds);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("LogCompensate");
    }
}