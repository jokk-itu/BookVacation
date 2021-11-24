using System;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace BookVacationService.CourierActivities
{
    public class BookHotelActivity : IActivity<BookHotelArguments, BookHotelLog>
    {
        private readonly IRequestClient<BookHotel> _client;
        private readonly ILogger<BookHotelActivity> _logger;

        public BookHotelActivity(IRequestClient<BookHotel> client, ILogger<BookHotelActivity> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<BookHotelArguments> context)
        {
            _logger.LogInformation("Executing BookHotel");
            var price = context.Arguments.Price;
            var bookHotelId = NewId.NextGuid();

            var response = await _client.GetResponse<BookedHotel>(new
            {
                BookHotelId = bookHotelId,
                Price = price
            });

            _logger.LogInformation("Executed BookHotel");
            return context.Completed(new { HotelId = bookHotelId });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<BookHotelLog> context)
        {
            await Task.Delay(500);
            _logger.LogInformation("RentCar Compensated {Log}", JsonSerializer.Serialize(context.Log));
            return context.Compensated();
        }
    }

    public interface BookHotelArguments
    {
        public decimal Price { get; }
    }

    public interface BookHotelLog
    {
        public Guid HotelId { get; }
    }
}