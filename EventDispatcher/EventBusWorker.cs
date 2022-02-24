using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventDispatcher;

public class EventBusWorker : BackgroundService
{
    private readonly IBusControl _busControl;
    private readonly ILogger<EventBusWorker> _logger;

    public EventBusWorker(ILogger<EventBusWorker> logger, IBusControl busControl)
    {
        _logger = logger;
        _busControl = busControl;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _busControl.StartAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
        _logger.LogDebug("Bus started");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _busControl.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
        _logger.LogDebug("Bus stopped");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested) await Task.Delay(1000, stoppingToken);
    }
}