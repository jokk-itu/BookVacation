using System.Diagnostics;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookFlightService;

public class LogSendFilter<T> : IFilter<SendContext<T>> where T: class
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
        _logger.LogInformation("Sent message, took {Elapsed}", watch.ElapsedMilliseconds);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("SendLog");
    }
}