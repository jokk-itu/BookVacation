using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Filters.Log;

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
        var watch = Stopwatch.StartNew();
        await next.Send(context);
        watch.Stop();

        var scope = new Dictionary<string, object>
        {
            { "TrackingNumber", context.TrackingNumber },
            { "ActivityName", context.ActivityName }
        };
        if (context.MessageId is not null)
            scope.Add("MessageId", context.MessageId);

        using (_logger.BeginScope(scope))
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