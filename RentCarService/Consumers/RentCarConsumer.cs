using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace RentCarService.Consumers
{
    public class RentCarConsumer : IConsumer<RentCar>
    {
        private readonly ILogger<RentCarConsumer> _logger;

        public RentCarConsumer(ILogger<RentCarConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RentCar> context)
        {
            _logger.LogInformation("Received RentCar message");
            var rentCarId = context.Message.RentCarId;
            var price = context.Message.Price;

            await context.RespondAsync<RentedCar>(new
            {
                RentCarId = rentCarId,
                Price = price
            });

            _logger.LogInformation("Replied RentedCar");
        }
    }
}