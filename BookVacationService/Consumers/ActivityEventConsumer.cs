using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;

namespace BookVacationService.Consumers
{
    public class ActivityEventConsumer :
        IConsumer<RoutingSlipFaulted>,
        IConsumer<RoutingSlipCompleted>,
        IConsumer<RoutingSlipActivityCompleted>,
        IConsumer<RoutingSlipActivityCompensated>,
        IConsumer<RoutingSlipActivityFaulted>
    {
        private readonly ILogger<ActivityEventConsumer> _logger;

        public ActivityEventConsumer(ILogger<ActivityEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RoutingSlipActivityCompensated> context)
        {
            _logger.LogInformation("RoutingSlip Activity Compensated");
        }

        public async Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
        {
            _logger.LogInformation("RoutingSlip Activity Completed");
        }

        public async Task Consume(ConsumeContext<RoutingSlipActivityFaulted> context)
        {
            _logger.LogInformation("RoutingSlip Activity Faulted");
        }

        public async Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            _logger.LogInformation("RoutingSlip Completed");
        }

        public async Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            _logger.LogInformation("RoutingSlip Faulted");
        }
    }
}