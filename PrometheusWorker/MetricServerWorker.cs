using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PrometheusWorker;

public class MetricServerWorker : BackgroundService
{
    private readonly ILogger<MetricServerWorker> _logger;

    public MetricServerWorker(ILogger<MetricServerWorker> logger)
    {
        _logger = logger;
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        _logger.LogDebug("Started MetricServer");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _logger.LogDebug("Stopped MetricServer");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }
}