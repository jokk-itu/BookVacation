using System.Diagnostics;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventBusTransmitting.Filters;

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
        var watch = new Stopwatch();
        watch.Start();
        await next.Send(context);
        watch.Stop();
        _logger.LogInformation("Sent {Message} with {MessageId}, took {Elapsed} ms", context.Message.GetType().Name,
            context.MessageId, watch.ElapsedMilliseconds);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("SendLog");
    }
}