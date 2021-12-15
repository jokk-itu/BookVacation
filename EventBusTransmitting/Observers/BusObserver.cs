using MassTransit;
using Microsoft.Extensions.Logging;

namespace EventBusTransmitting.Observers;

public class BusObserver : IBusObserver
{
    private readonly ILogger<BusObserver> _logger;

    public BusObserver(ILogger<BusObserver> logger)
    {
        _logger = logger;
    }

    public void PostCreate(IBus bus)
    {
        _logger.LogDebug("");
    }

    public void CreateFaulted(Exception exception)
    {
        _logger.LogDebug("");
    }

    public Task PreStart(IBus bus)
    {
        _logger.LogDebug("");
        return Task.CompletedTask;
    }

    public Task PostStart(IBus bus, Task<BusReady> busReady)
    {
        _logger.LogDebug("PostStart");
        return Task.CompletedTask;
    }

    public Task StartFaulted(IBus bus, Exception exception)
    {
        _logger.LogDebug("FaultStart");
        return Task.CompletedTask;
    }

    public Task PreStop(IBus bus)
    {
        _logger.LogDebug("PreStop");
        return Task.CompletedTask;
    }

    public Task PostStop(IBus bus)
    {
        _logger.LogDebug("PostStop");
        return Task.CompletedTask;
    }

    public Task StopFaulted(IBus bus, Exception exception)
    {
        _logger.LogDebug("FaultStop");
        return Task.CompletedTask;
    }
}