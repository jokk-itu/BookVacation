using FlightService.Domain.Entities.Nodes;
using MediatR;
using Neo4j.Driver;

namespace FlightService.Infrastructure.Requests.ReadFlight;

public class ReadFlightRequestHandler : IRequestHandler<ReadFlightRequest, (RequestResult, Flight?)>
{
    private readonly IDriver _driver;

    public ReadFlightRequestHandler(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<(RequestResult, Flight?)> Handle(ReadFlightRequest request, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        var flight = await session.ReadTransactionAsync(async transaction =>
        {
            const string query = @"
MATCH (f:Flight {id: $id})
RETURN f";
            var result = await transaction.RunAsync(query, new { id = request.Id.ToString() });
            if (!await result.FetchAsync())
                return null;

            return new Flight
            {
                Id = (Guid)result.Current["id"],
                From = (DateTime)result.Current["from"],
                To = (DateTime)result.Current["to"]
            };
        });
        return flight is null
            ? (RequestResult.NotFound, null)
            : (RequestResult.Ok, flight);
    }
}