using System;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace RentCarService.CourierActivities
{
    public class RentCarActivity : IActivity<RentCarArguments, RentCarLog>
    {
        private readonly IRequestClient<RentCar> _client;
        private readonly ILogger<RentCarActivity> _logger;

        public RentCarActivity(IRequestClient<RentCar> client, ILogger<RentCarActivity> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<RentCarArguments> context)
        {
            _logger.LogInformation("Executing RentCar");
            var price = context.Arguments.Price;
            var rentCarId = NewId.NextGuid();

            var response = await _client.GetResponse<RentedCar>(new
            {
                RentCarId = rentCarId,
                Price = price
            });

            _logger.LogInformation("Executed RentCar");
            return context.Completed(new { RentCarId = rentCarId });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<RentCarLog> context)
        {
            await Task.Delay(500);
            _logger.LogInformation("RentCar Compensated {Log}", JsonSerializer.Serialize(context.Log));
            return context.Compensated();
        }
    }

    public interface RentCarArguments
    {
        public decimal Price { get; }
    }

    public interface RentCarLog
    {
        public Guid RentCarId { get; }
    }
}