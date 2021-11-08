using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookFlightService.Consumers
{
    public class BookFlightConsumer : IConsumer<BookFlight>
    {
        private readonly ILogger<BookFlightConsumer> _logger;
        private readonly IMessageScheduler _scheduler;

        public BookFlightConsumer(ILogger<BookFlightConsumer> logger, IMessageScheduler scheduler)
        {
            _logger = logger;
            _scheduler = scheduler;
        }

        public async Task Consume(ConsumeContext<BookFlight> context)
        {
            _logger.LogInformation("Received BookFlight message");
            var bookFlightId = context.Message.BookFlightId;
            var price = context.Message.Price;
            await _scheduler.SchedulePublish<BookHotel>(TimeSpan.FromSeconds(10),
                new
                {
                    __CorrelationId = Guid.NewGuid(),
                    BookHotelId = bookFlightId,
                    Price = price
                });

            await context.RespondAsync<BookedFlight>(new
            {
                BookFlightId = bookFlightId,
                Price = price
            });
            _logger.LogInformation("Replied BookedFlight");
        }
    }
}