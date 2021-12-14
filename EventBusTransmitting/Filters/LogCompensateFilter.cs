using System.Diagnostics;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace BookFlightService;

public class LogCompensateFilter<T> : IFilter<CompensateContext<T>> where T: class
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
        _logger.LogInformation("Compensated message, took {Elapsed}", watch.ElapsedMilliseconds);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("LogCompensate");
    }
}