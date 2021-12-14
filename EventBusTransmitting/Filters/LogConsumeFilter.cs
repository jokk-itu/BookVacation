using System.Diagnostics;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookFlightService;

public class LogConsumeFilter<T> : IFilter<ConsumeContext<T>> where T: class
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
        _logger.LogInformation("Consumed message, took {Elapsed}", watch.ElapsedMilliseconds);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("ConsumeLog");
    }
}