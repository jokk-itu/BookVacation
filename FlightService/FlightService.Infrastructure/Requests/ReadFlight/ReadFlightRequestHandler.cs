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
RETURN f.id AS id, f.from AS from, f.to as to";
            var result = await transaction.RunAsync(query, new { id = request.Id.ToString() });
            if (!await result.FetchAsync())
                return null;

            return new Flight
            {
                Id = Guid.Parse(result.Current.Values["id"].ToString()),
                From = DateTime.Parse(result.Current.Values["from"].ToString()),
                To = DateTime.Parse(result.Current.Values["to"].ToString())
            };
        });
        return flight is null
            ? (RequestResult.NotFound, null)
            : (RequestResult.Ok, flight);
    }
}