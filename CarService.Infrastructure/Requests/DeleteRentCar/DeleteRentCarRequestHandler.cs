using MediatR;
using Neo4j.Driver;

namespace CarService.Infrastructure.Requests.DeleteRentCar;

public class DeleteRentCarRequestHandler : IRequestHandler<DeleteRentCarRequest, RequestResult>
{
    private readonly IDriver _driver;

    public DeleteRentCarRequestHandler(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<RequestResult> Handle(DeleteRentCarRequest request, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (r:Rent {id: $rentCarId})
DETACH DELETE r";
            var result = await transaction.RunAsync(command, new
            {
                rentId = request.RentCarId.ToString()
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