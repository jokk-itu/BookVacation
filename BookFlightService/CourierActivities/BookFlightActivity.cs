using System.Diagnostics;
using System.Threading.Tasks;
using Contracts.BookFlightActivity;
using MassTransit;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace BookFlightService.CourierActivities;

public class BookFlightActivity : IActivity<BookFlightArgument, BookFlightLog>
{
    private readonly IDriver _driver;
    private readonly ILogger<BookFlightActivity> _logger;

    public BookFlightActivity(ILogger<BookFlightActivity> logger, IDriver driver)
    {
        _logger = logger;
        _driver = driver;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<BookFlightArgument> context)
    {
        var seatId = context.Arguments.SeatId;
        var flightId = context.Arguments.FlightId;
        var reservationId = NewId.NextGuid();

        await using var session = _driver.AsyncSession();
        var watch = new Stopwatch();
        watch.Start();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (f:Flight {id: $flightId})-[:With]->(ap:Airplane)-[:Has]->(s:Seat {id: $seatId})
WHERE NOT EXISTS {
    MATCH 
        (:Reservation)-->(f),
        (:Reservation)-->(s)
}
CREATE (r1:Reservation {id: $reservationId})-[:Reserves]->(s)
CREATE (r1)-[:Buys]->(f)
RETURN true AS IsSuccessful";
            var result = await transaction.RunAsync(command,
                new
                {
                    flightId = flightId.ToString(),
                    seatId,
                    reservationId = reservationId.ToString()
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
        watch.Stop();
        _logger.LogInformation("Executed BookFlight, took {Elapsed}", watch.ElapsedMilliseconds);
        return isSuccessful ? context.Completed(new { ReservationId = reservationId }) : context.Faulted();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<BookFlightLog> context)
    {
        var reservationId = context.Log.ReservationId;

        await using var session = _driver.AsyncSession();
        var watch = new Stopwatch();
        watch.Start();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (r:Reservation {id: $id})
DETACH DELETE r
RETURN true AS IsSuccessful";
            var result = await transaction.RunAsync(command,
                new
                {
                    id = reservationId.ToString()
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
        watch.Stop();
        _logger.LogInformation("Compensated BookFlight, took {Elapsed}", watch.ElapsedMilliseconds);
        return isSuccessful ? context.Compensated() : context.Failed();
    }
}