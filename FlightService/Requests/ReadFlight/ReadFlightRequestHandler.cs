using FlightService.Entities.Nodes;
using MediatR;
using Neo4j.Driver;

namespace FlightService.Requests.ReadFlight;

public class ReadFlightRequestHandler : IRequestHandler<ReadFlightRequest, (Response, Flight)>
{
    private readonly IDriver _driver;

    public ReadFlightRequestHandler(IDriver driver)
    {
        _driver = driver;
    }
    
    public async Task<(Response, Flight)> Handle(ReadFlightRequest request, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        var flight = await session.ReadTransactionAsync(async transaction =>
        {
            const string query = @"
MATCH (f:Flight {id: $id})
RETURN f";
            var result = await transaction.RunAsync(query, new {id = request.Id});
            var record = await result.SingleAsync();
            return record.As<Flight>();
        });
        return (Response.Ok, flight);
    }
}