using DocumentClient;
using FlightService.Domain;
using Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace FlightService.Infrastructure.Requests.CreateFlight;

public class CreateFlightCommandHandler : ICommandHandler<CreateFlightCommand, Flight>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<CreateFlightCommandHandler> _logger;

    public CreateFlightCommandHandler(IDocumentClient client, ILogger<CreateFlightCommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<Flight>> Handle(CreateFlightCommand command, CancellationToken cancellationToken)
    {
        var airplane = await _client.QueryAsync<Airplane>(query => query
            .Where(x => x.Id == command.AirplaneId.ToString())
            .FirstOrDefaultAsync(cancellationToken));

        if (airplane is null)
        {
            _logger.LogDebug("Airplane with identifier {Identifier}, does not exist",
                command.AirplaneId);
            return new Response<Flight>(ResponseCode.NotFound,
                new[] { "Airplane does not exist" });
        }

        var conflictingFlight = await _client.QueryAsync<Flight>(query => query
            .Where(x => x.AirPlaneId == command.AirplaneId)
            .Where(x => (command.From >= x.From && command.From <= x.To) ||
                        (command.To >= x.From && command.To <= x.To))
            .FirstOrDefaultAsync(cancellationToken));

        if (conflictingFlight is not null)
        {
            _logger.LogDebug("Airplane is in use by identifier {Identifier}",
                conflictingFlight.Id);
            return new Response<Flight>(ResponseCode.Conflict,
                new[] { "Airplane is in use" });
        }

        var flight = new Flight
        {
            From = command.From,
            To = command.To,
            FromAirport = command.FromAirport,
            ToAirport = command.ToAirport,
            AirPlaneId = command.AirplaneId,
            Price = command.Price
        };
        await _client.StoreAsync(flight, cancellationToken);
        return new Response<Flight>(flight);
    }
}