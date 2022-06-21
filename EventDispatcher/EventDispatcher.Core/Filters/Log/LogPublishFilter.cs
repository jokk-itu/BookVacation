using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Filters.Log;

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
        var watch = Stopwatch.StartNew();
        await next.Send(context);
        watch.Stop();

        var scope = new Dictionary<string, object>();
        if (context.MessageId is not null)
            scope.Add("MessageId", context.MessageId);

        using (_logger.BeginScope(scope))
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