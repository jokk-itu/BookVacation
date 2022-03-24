using System.ComponentModel.Design;
using System.Diagnostics;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Filters;

public class LogPublishFilter<T> : IFilter<PublishContext<T>> where T : class
{
    private readonly ILogger<LogPublishFilter<T>> _logger;

    public LogPublishFilter(ILogger<LogPublishFilter<T>> logger)
    {
        _logger = logger;
    }

    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        _logger.LogDebug("Publishing message");
        var watch = new Stopwatch();
        watch.Start();
        await next.Send(context);
        watch.Stop();
        using (_logger.BeginScope(new Dictionary<string, object>
                   { { "MessageId", context.MessageId }, { "CorrelationId", context.CorrelationId } }))
        {
            _logger.LogInformation("Published {Message} to {Destination}, took {Elapsed} ms",
                context.Message.GetType().Name, context.DestinationAddress, watch.ElapsedMilliseconds);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("PublishScope");
    }
}