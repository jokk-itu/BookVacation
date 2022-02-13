using MediatR;
using Neo4j.Driver;

namespace CarService.Requests.CreateRentCar;

public class CreateRentCarRequestHandler : IRequestHandler<CreateRentCarRequest, RequestResult>
{
    private readonly IDriver _driver;

    public CreateRentCarRequestHandler(IDriver driver)
    {
        _driver = driver;
    }
    
    public async Task<RequestResult> Handle(CreateRentCarRequest request, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
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
CREATE (r)-[:RentingFor]->(rc)";
            var result = await transaction.RunAsync(command, new
            {
                companyId = request.CompanyId.ToString(),
                carId = request.CarId.ToString(),
                days = request.Days,
                rentId = request.RentId.ToString()
            });
            var isSuccessful = await result.FetchAsync();

            if (isSuccessful)
            {
                await transaction.CommitAsync();
                return true;
            }

            await transaction.RollbackAsync();
            return false;
        });
        return isSuccessful 
            ? RequestResult.Ok 
            : RequestResult.Error;
    }
}