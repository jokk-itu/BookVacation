using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventBusTransmitting.Observers;

public class PublishObserver : IPublishObserver
{
    private readonly ILogger<PublishObserver> _logger;

    public PublishObserver(ILogger<PublishObserver> logger)
    {
        _logger = logger;
    }

    public Task PrePublish<T>(PublishContext<T> context) where T : class
    {
        _logger.LogDebug("");
        return Task.CompletedTask;
    }

    public Task PostPublish<T>(PublishContext<T> context) where T : class
    {
        _logger.LogDebug("");
        return Task.CompletedTask;
    }

    public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
    {
        _logger.LogDebug("");
        return Task.CompletedTask;
    }
}