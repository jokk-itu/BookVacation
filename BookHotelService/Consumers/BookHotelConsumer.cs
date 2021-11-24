using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookHotelService.Consumers
{
    public class BookHotelConsumer : IConsumer<BookHotel>
    {
        private readonly ILogger<BookHotelConsumer> _logger;

        public BookHotelConsumer(ILogger<BookHotelConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BookHotel> context)
        {
            _logger.LogInformation("Received BookHotel message");
            var bookHotelId = context.Message.HotelId;
            var price = context.Message.Price;

            await context.RespondAsync<BookedHotel>(new
            {
                BookHotelId = bookHotelId,
                Price = price
            });

            _logger.LogInformation("Replied BookedHotel");
        }
    }
}