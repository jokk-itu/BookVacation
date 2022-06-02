using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Filters;

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
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   { "MessageId", context.MessageId }, { "TrackingNumber", context.TrackingNumber },
                   { "CorrelationId", context.CorrelationId }, { "ActivityName", context.ActivityName }
               }))
        {
            _logger.LogInformation("Compensated {Message}, took {Elapsed} ms",
                context.Message.GetType().Name, watch.ElapsedMilliseconds);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("CompensateScope");
    }
}