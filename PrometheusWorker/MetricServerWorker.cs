using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus.Client.MetricServer;

namespace PrometheusWorker;

internal class MetricServerWorker : BackgroundService
{
    private readonly ILogger<MetricServerWorker> _logger;
    private readonly IMetricServer _metricServer;

    public MetricServerWorker(ILogger<MetricServerWorker> logger, IMetricServer metricServer)
    {
        _logger = logger;
        _metricServer = metricServer;
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _metricServer.Start();
        await base.StartAsync(cancellationToken);
        _logger.LogDebug("Started MetricServer");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _metricServer.Stop();
        await base.StopAsync(cancellationToken);
        _logger.LogDebug("Stopped MetricServer");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }
}