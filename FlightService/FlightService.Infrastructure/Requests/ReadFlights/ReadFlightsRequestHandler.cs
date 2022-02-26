using FlightService.Domain.Entities.Nodes;
using MediatR;
using Neo4j.Driver;

namespace FlightService.Infrastructure.Requests.ReadFlights;

public class ReadFlightsRequestHandler : IRequestHandler<ReadFlightsRequest, (RequestResult, IEnumerable<Flight>?)>
{
    private readonly IDriver _driver;

    public ReadFlightsRequestHandler(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<(RequestResult, IEnumerable<Flight>?)> Handle(ReadFlightsRequest request,
        CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        var flights = await session.ReadTransactionAsync(async transaction =>
        {
            const string query = @"
MATCH (f:Flight {id: $id})
RETURN f.id AS id, f.from AS from, f.to AS to
ORDER BY f.id
SKIP $offset
LIMIT $amount";
            var result = await transaction.RunAsync(query, new
            {
                offset = request.Offset,
                amount = request.Amount
            });

            if (!await result.FetchAsync())
                return null;

            var flights = new List<Flight>();
            do
            {
                flights.Add(new Flight
                {
                    Id = (Guid)result.Current["id"],
                    From = (DateTime)result.Current["from"],
                    To = (DateTime)result.Current["to"]
                });
            } while (await result.FetchAsync());

            return flights;
        });

        return flights is null
            ? (RequestResult.NotFound, null)
            : (RequestResult.Ok, flights);
    }
}