using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Filters.Log;

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
        var watch = Stopwatch.StartNew();
        await next.Send(context);
        watch.Stop();

        var scope = new Dictionary<string, object>();
        if (context.MessageId is not null)
            scope.Add("MessageId", context.MessageId);

        using (_logger.BeginScope(scope))
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