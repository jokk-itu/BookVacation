using MediatR;
using Neo4j.Driver;

namespace FlightService.Infrastructure.Requests.DeleteBookFlight;

public class DeleteBookFlightRequestHandler : IRequestHandler<DeleteBookFlightRequest, RequestResult>
{
    private readonly IDriver _driver;

    public DeleteBookFlightRequestHandler(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<RequestResult> Handle(DeleteBookFlightRequest request, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (r:Reservation {id: $id})
DETACH DELETE r
RETURN true AS IsSuccessful";
            var result = await transaction.RunAsync(command,
                new
                {
                    id = request.ReservationId.ToString()
                });

            if (await result.FetchAsync())
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