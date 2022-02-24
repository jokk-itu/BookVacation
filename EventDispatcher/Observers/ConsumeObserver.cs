using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Observers;

public class ConsumeObserver : IConsumeObserver
{
    private readonly ILogger<ConsumeObserver> _logger;

    public ConsumeObserver(ILogger<ConsumeObserver> logger)
    {
        _logger = logger;
    }

    public Task PreConsume<T>(ConsumeContext<T> context) where T : class
    {
        _logger.LogDebug("PreConsume");
        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context) where T : class
    {
        _logger.LogDebug("PostConsume");
        return Task.CompletedTask;
    }

    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
    {
        _logger.LogDebug("FaultConsume");
        return Task.CompletedTask;
    }
}