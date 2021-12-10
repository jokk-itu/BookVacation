using System.Text.Json;
using System.Threading.Tasks;
using Contracts.BookFlightActivity;
using MassTransit;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace BookFlightService.CourierActivities;

public class BookFlightActivity : IActivity<BookFlightArgument, BookFlightLog>
{
    private readonly ILogger<BookFlightActivity> _logger;
    private readonly IDriver _driver;

    public BookFlightActivity(ILogger<BookFlightActivity> logger, IDriver driver)
    {
        _logger = logger;
        _driver = driver;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<BookFlightArgument> context)
    {
        _logger.LogInformation("Executing BookFlight");
        var seatId = context.Arguments.SeatId;
        var flightId = context.Arguments.FlightId;
        var reservationId = NewId.NextGuid();

        var session = _driver.AsyncSession();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (f:Flight {id: $flightId})-[:With]->(ap:Airplane)-[:Has]->(s:Seat {id: $seatId})
WHERE NOT EXISTS {
    MATCH (r:Reservation)-->(f)
}
CREATE (r1:Reservation {id: $reservationId})-[:Reserves]->(s)
CREATE (r1)-[:Buys]->(f)
RETURN true AS IsSuccessful";
            var result = await transaction.RunAsync(command,
                new
                {
                    flightId = flightId.ToString().ToUpper(),
                    seatId,
                    reservationId = reservationId.ToString().ToUpper()
                });
            var record = await result.FetchAsync();

            if (record)
            {
                await transaction.CommitAsync();
                return true;
            }

            await transaction.RollbackAsync();
            return false;
        });

        _logger.LogInformation("Executed BookFlight");

        return isSuccessful ? context.Completed() : context.Faulted();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<BookFlightLog> context)
    {
        await Task.Delay(500);
        _logger.LogInformation("RentCar Compensated {Log}", JsonSerializer.Serialize(context.Log));
        return context.Compensated();
    }
}