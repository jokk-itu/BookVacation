using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Filters;

public class LogConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly ILogger<LogConsumeFilter<T>> _logger;

    public LogConsumeFilter(ILogger<LogConsumeFilter<T>> logger)
    {
        _logger = logger;
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        _logger.LogDebug("Consuming message");
        var watch = new Stopwatch();
        watch.Start();
        await next.Send(context);
        watch.Stop();
        using (_logger.BeginScope(new Dictionary<string, object>
                   { { "MessageId", context.MessageId }, { "CorrelationId", context.CorrelationId } }))
        {
            _logger.LogInformation("Consumed {Message}, took {Elapsed} ms", context.Message.GetType().Name,
                watch.ElapsedMilliseconds);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("ConsumeScope");
    }
}