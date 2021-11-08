using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookFlightService
{
    public class Worker : BackgroundService
    {
        private readonly IBusControl _busControl;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _busControl.StartAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _busControl.StopAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) await Task.Delay(1000, stoppingToken);
        }
    }
}