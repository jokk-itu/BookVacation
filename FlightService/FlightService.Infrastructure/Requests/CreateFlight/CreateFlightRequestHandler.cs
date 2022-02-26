using FlightService.Domain.Entities.Nodes;
using MediatR;
using Neo4j.Driver;

namespace FlightService.Infrastructure.Requests.CreateFlight;

public class CreateFlightRequestHandler : IRequestHandler<CreateFlightRequest, (RequestResult, Flight?)>
{
    private readonly IDriver _driver;

    public CreateFlightRequestHandler(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<(RequestResult, Flight?)> Handle(CreateFlightRequest request, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        var flight = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
CREATE (f:Flight {id: $id, from: $from, to: $to})
RETURN f.id AS id, f.from AS from, f.to AS to";
            var result = await transaction.RunAsync(command, new
            {
                id = Guid.NewGuid().ToString(),
                from = request.From,
                to = request.To
            });

            if (await result.FetchAsync())
            {
                var flight = new Flight
                {
                    Id = Guid.Parse(result.Current.Values["id"].ToString()),
                    From = DateTime.Parse(result.Current.Values["from"].ToString()),
                    To = DateTime.Parse(result.Current.Values["to"].ToString())
                };
                await transaction.CommitAsync();
                return flight;
            }

            await transaction.RollbackAsync();
            return null;
        });

        return flight is null ? (RequestResult.Error, null) : (RequestResult.Created, flight);
    }
}