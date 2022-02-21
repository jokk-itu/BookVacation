using FlightService.Domain.Entities.Nodes;
using FlightService.Infrastructure.Requests.ReadFlight;
using MediatR;
using Neo4j.Driver;

namespace FlightService.Infrastructure.Requests.CreateFlight;

public class CreateFlightRequestHandler : IRequestHandler<CreateFlightRequest, (RequestResult, Flight?)>
{
    private readonly IDriver _driver;
    private readonly IMediator _mediator;

    public CreateFlightRequestHandler(IDriver driver, IMediator mediator)
    {
        _driver = driver;
        _mediator = mediator;
    }

    public async Task<(RequestResult, Flight?)> Handle(CreateFlightRequest request, CancellationToken cancellationToken)
    {
        if (await IsFlightConflicting(cancellationToken))
            return (RequestResult.Conflict, null);

        await using var session = _driver.AsyncSession();
        var flight = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
CREATE (f:Flight {id: $id, from: $from, to: $to}
RETURN f)";
            var result = await transaction.RunAsync(command, new
            {
                id = Guid.NewGuid(),
                from = request.From,
                to = request.To
            });
            var isSuccessful = await result.FetchAsync();

            if (isSuccessful)
            {
                await transaction.CommitAsync();
                return result.SingleAsync().As<Flight>();
            }

            await transaction.RollbackAsync();
            return null;
        });

        return flight is null ? (RequestResult.Error, null) : (RequestResult.Created, flight);
    }

    private async Task<bool> IsFlightConflicting(CancellationToken cancellationToken)
    {
        var (result, _) = await _mediator.Send(new ReadFlightRequest(Guid.Empty), cancellationToken);
        return result == RequestResult.Conflict;
    }
}