using System.Diagnostics;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventBusTransmitting.Filters;

public class LogPublishFilter<T> : IFilter<PublishContext<T>> where T: class
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
        _logger.LogInformation("Published message, took {Elapsed}", watch.ElapsedMilliseconds);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("PublishScope");
    }
}