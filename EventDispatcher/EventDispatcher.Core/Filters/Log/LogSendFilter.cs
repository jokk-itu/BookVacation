using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Filters;

public class LogSendFilter<T> : IFilter<SendContext<T>> where T : class
{
    private readonly ILogger<LogSendFilter<T>> _logger;

    public LogSendFilter(ILogger<LogSendFilter<T>> logger)
    {
        _logger = logger;
    }

    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        _logger.LogDebug("Sending message");
        var watch = Stopwatch.StartNew();
        await next.Send(context);
        watch.Stop();
        using (_logger.BeginScope(new Dictionary<string, object>
                   { { "MessageId", context.MessageId }, { "CorrelationId", context.CorrelationId } }))
        {
            _logger.LogInformation("Sent {Message} to {Destination}, took {Elapsed} ms", context.Message.GetType().Name,
                context.DestinationAddress, watch.ElapsedMilliseconds);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("SendScope");
    }
}