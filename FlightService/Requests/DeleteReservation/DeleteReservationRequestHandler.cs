using MediatR;
using Neo4j.Driver;

namespace FlightService.Requests.DeleteReservation;

public class DeleteReservationRequestHandler : IRequestHandler<DeleteReservationRequest, RequestResult>
{
    private readonly IDriver _driver;

    public DeleteReservationRequestHandler(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<RequestResult> Handle(DeleteReservationRequest request, CancellationToken cancellationToken)
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
            var record = await result.FetchAsync();

            if (record)
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