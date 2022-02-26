using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventDispatcher.Observers;

public class SendObserver : ISendObserver
{
    private readonly ILogger<SendObserver> _logger;

    public SendObserver(ILogger<SendObserver> logger)
    {
        _logger = logger;
    }

    public Task PreSend<T>(SendContext<T> context) where T : class
    {
        _logger.LogDebug("PreSend");
        return Task.CompletedTask;
    }

    public Task PostSend<T>(SendContext<T> context) where T : class
    {
        _logger.LogDebug("PostSend");
        return Task.CompletedTask;
    }

    public Task SendFault<T>(SendContext<T> context, Exception exception) where T : class
    {
        _logger.LogDebug("FaultSend");
        return Task.CompletedTask;
    }
}