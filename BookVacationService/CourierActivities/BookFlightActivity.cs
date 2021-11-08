using System;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace BookVacationService.CourierActivities
{
    public class BookFlightActivity : IActivity<BookFlightArguments, BookFlightLog>
    {
        private readonly IRequestClient<BookFlight> _client;
        private readonly ILogger<BookFlightActivity> _logger;

        public BookFlightActivity(IRequestClient<BookFlight> client, ILogger<BookFlightActivity> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<BookFlightArguments> context)
        {
            _logger.LogInformation("Executing BookFlight");
            var price = context.Arguments.Price;
            var bookFlightId = NewId.NextGuid();

            var response = await _client.GetResponse<BookedFlight>(new
            {
                BookFlightId = bookFlightId,
                Price = price
            });

            _logger.LogInformation("Executed BookFlight");

            return context.Completed(new { BookFlightId = bookFlightId });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<BookFlightLog> context)
        {
            await Task.Delay(500);
            _logger.LogInformation("RentCar Compensated {Log}", JsonSerializer.Serialize(context.Log));
            return context.Compensated();
        }
    }

    public interface BookFlightArguments
    {
        public decimal Price { get; }
    }

    public interface BookFlightLog
    {
        public Guid BookFlightId { get; }
    }
}