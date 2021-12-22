using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventBusTransmitting.Observers;

public class ReceiveObserver : IReceiveObserver
{
    private readonly ILogger<ReceiveObserver> _logger;

    public ReceiveObserver(ILogger<ReceiveObserver> logger)
    {
        _logger = logger;
    }

    public Task PreReceive(ReceiveContext context)
    {
        _logger.LogDebug("PreReceive");
        return Task.CompletedTask;
    }

    public Task PostReceive(ReceiveContext context)
    {
        _logger.LogDebug("PostReceive");
        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
    {
        _logger.LogDebug("PostConsume");
        return Task.CompletedTask;
    }

    public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        where T : class
    {
        _logger.LogDebug("FaultConsume");
        return Task.CompletedTask;
    }

    public Task ReceiveFault(ReceiveContext context, Exception exception)
    {
        _logger.LogDebug("FaultReceive");
        return Task.CompletedTask;
    }
}