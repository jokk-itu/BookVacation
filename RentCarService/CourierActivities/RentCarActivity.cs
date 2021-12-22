using System.Diagnostics;
using Contracts.RentCarActivity;
using MassTransit;
using MassTransit.Courier;
using Neo4j.Driver;

namespace RentCarService.CourierActivities;

public class RentCarActivity : IActivity<RentCarArgument, RentCarLog>
{
    private readonly IDriver _driver;
    private readonly ILogger<RentCarActivity> _logger;

    public RentCarActivity(ILogger<RentCarActivity> logger, IDriver driver)
    {
        _logger = logger;
        _driver = driver;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<RentCarArgument> context)
    {
        var companyId = context.Arguments.RentingCompanyId;
        var carId = context.Arguments.CarId;
        var days = context.Arguments.Days;
        var rentId = NewId.NextGuid();

        await using var session = _driver.AsyncSession();
        var watch = new Stopwatch();
        watch.Start();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (rc:RentingCompany {id: $companyId})-[:Owns]->(c:Car {id: $carId})
WHERE NOT EXISTS {
    MATCH 
        (:RentCar)-->(rc),
        (:RentCar)-->(c)
}
CREATE (r:RentCar {id: $rentId})-[:Renting]->(:Car {id: $carId})
CREATE (r)-[:RentingFor]->(rc)
RETURN true as IsSuccessful";
            var result = await transaction.RunAsync(command, new
            {
                companyId = companyId.ToString(),
                carId = carId.ToString(),
                days,
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
        _logger.LogInformation("Executed RentCar, took {Elapsed}", watch.ElapsedMilliseconds);
        return isSuccessful ? context.Completed(new { RentId = rentId }) : context.Faulted();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<RentCarLog> context)
    {
        var rentId = context.Log.RentCarId;

        await using var session = _driver.AsyncSession();
        var watch = new Stopwatch();
        watch.Start();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (r:Rent {id: $rentCarId})
DETACH DELETE r
RETURN true as IsSuccessful";
            var result = await session.RunAsync(command, new
            {
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
        _logger.LogInformation("Compensated RentCar, took {Elapsed}", watch.ElapsedMilliseconds);
        return isSuccessful ? context.Compensated() : context.Failed();
    }
}