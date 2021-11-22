using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace BookVacationService.Consumers
{
    public class BookVacationConsumer : IConsumer<BookVacation>
    {
        private readonly ILogger<BookVacationConsumer> _logger;

        public BookVacationConsumer(ILogger<BookVacationConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BookVacation> context)
        {
            _logger.LogInformation("Received Book Vacation");

            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            var bookFlight = context.Message.BookFlight;

            var bookHotel = context.Message.BookHotel;

            var rentCar = context.Message.RentCar;

            builder.AddActivity("BookFlight",
                new Uri("queue:book-flight_execute"), bookFlight);

            builder.AddActivity("BookHotel",
                new Uri("queue:book-hotel_execute"), bookHotel);

            builder.AddActivity("RentCar",
                new Uri("queue:rent-car_execute"), rentCar);

            var routingSlip = builder.Build();

            _logger.LogInformation("Executing Book Vacation");

            await context.Execute(routingSlip);

            _logger.LogInformation("Executed Book Vacation");


            _logger.LogInformation("Execute CreateBookFlight");
            var correlationId = NewId.NextGuid();
            await context.Publish<CreateBookFlight>(new
            {
                __CorrelationId = correlationId,
                bookFlight.BookFlightId,
                bookFlight.Price
            });
            await Task.Delay(5000);
            
            /*_logger.LogInformation("Execute CompleteBookFlight");
            await context.Publish<CompleteBookFlight>(new
            {
                __CorrelationId = correlationId,
                bookFlight.BookFlightId,
                bookFlight.Price
            });*/
        }
    }
}