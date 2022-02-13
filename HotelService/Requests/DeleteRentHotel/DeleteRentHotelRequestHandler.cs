using MediatR;
using Neo4j.Driver;

namespace HotelService.Requests.DeleteRentHotel;

public class DeleteRentHotelRequestHandler : IRequestHandler<DeleteRentHotelRequest, RequestResult>
{
    private readonly IDriver _driver;

    public DeleteRentHotelRequestHandler(IDriver driver)
    {
        _driver = driver;
    }
    
    public async Task<RequestResult> Handle(DeleteRentHotelRequest request, CancellationToken cancellationToken)
    {
        var session = _driver.AsyncSession();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (r:Rent {id: $id})
DETACH DELETE r";
            var result = await transaction.RunAsync(command, new
            {
                id = request.RentId.ToString()
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