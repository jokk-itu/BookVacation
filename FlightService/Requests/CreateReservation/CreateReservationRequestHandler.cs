using MediatR;
using Neo4j.Driver;

namespace FlightService.Requests.CreateReservation;

public class CreateReservationRequestHandler : IRequestHandler<CreateReservationRequest, RequestResult>
{
    private readonly IDriver _driver;

    public CreateReservationRequestHandler(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<RequestResult> Handle(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
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
CREATE (r1)-[:Buys]->(f)";
            var result = await transaction.RunAsync(command,
                new
                {
                    flightId = request.FlightId.ToString(),
                    seatId = request.SeatId,
                    reservationId = request.ReservationId.ToString()
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
        return isSuccessful ? RequestResult.Error : RequestResult.Ok;
    }
}