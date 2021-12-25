using System.Diagnostics;
using Contracts.BookHotelActivity;
using MassTransit;
using MassTransit.Courier;
using Neo4j.Driver;

namespace HotelService.CourierActivities;

public class BookHotelActivity : IActivity<BookHotelArgument, BookHotelLog>
{
    private readonly IDriver _driver;
    private readonly ILogger<BookHotelActivity> _logger;

    public BookHotelActivity(ILogger<BookHotelActivity> logger, IDriver driver)
    {
        _logger = logger;
        _driver = driver;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<BookHotelArgument> context)
    {
        var days = context.Arguments.Days;
        var roomId = context.Arguments.RoomId;
        var hotelId = context.Arguments.HotelId;
        var rentId = NewId.NextGuid();

        await using var session = _driver.AsyncSession();
        var watch = new Stopwatch();
        watch.Start();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (h:Hotel {id: $hotelId})-[:Has]->(r:Room {id: $roomId})
WHERE NOT EXISTS {
    MATCH 
        (:Rent)-->(h),
        (:Rent)-->(r)    
}
CREATE (r1:Rent {id: $rentId})-[:At]->(h)
CREATE (r1)-[:Renting {days: $days}]->(r)
RETURN true as IsSuccessful
";
            var result = await transaction.RunAsync(command, new
            {
                days,
                roomId = roomId.ToString(),
                hotelId = hotelId.ToString(),
                rentId = rentId.ToString()
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
        _logger.LogInformation("Executed BookHotel, took {Elapsed}", watch.ElapsedMilliseconds);
        return isSuccessful ? context.Completed(new { RentId = rentId }) : context.Faulted();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<BookHotelLog> context)
    {
        var id = context.Log.RentId;
        var session = _driver.AsyncSession();
        var watch = new Stopwatch();
        watch.Start();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (r:Rent {id: $id})
DETACH DELETE r
RETURN true AS IsSuccessful";
            var result = await transaction.RunAsync(command, new
            {
                id = id.ToString()
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
        _logger.LogInformation("Compensated BookHotel, took {Elapsed}", watch.ElapsedMilliseconds);
        return isSuccessful ? context.Compensated() : context.Failed();
    }
}