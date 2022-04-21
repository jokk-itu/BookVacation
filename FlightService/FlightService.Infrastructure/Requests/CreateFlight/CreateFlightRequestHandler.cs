using DocumentClient;
using FlightService.Domain;
using MassTransit.Initializers.Variables;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace FlightService.Infrastructure.Requests.CreateFlight;

public class CreateFlightRequestHandler : IRequestHandler<CreateFlightRequest, Flight?>
{
    private readonly IDocumentClient _client;

    public CreateFlightRequestHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Flight?> Handle(CreateFlightRequest request, CancellationToken cancellationToken)
    {
        var airplane = await _client.QueryAsync<Airplane>(query => query
            .Where(x => x.Id == request.AirplaneId.ToString())
            .FirstOrDefaultAsync(cancellationToken));

        if (airplane is null)
            return null;

        var conflictingFlight = await _client.QueryAsync<Flight>(query => query
            .Where(x => x.AirPlaneId == request.AirplaneId)
            .Where(x => request.From >= x.From && request.From <= x.To ||
                        request.To >= x.From && request.To <= x.To)
            .FirstOrDefaultAsync(cancellationToken));

        if (conflictingFlight is not null)
            return null;

        var flight = new Flight
        {
            From = request.From,
            To = request.To,
            FromAirport = request.FromAirport,
            ToAirport = request.ToAirport,
            AirPlaneId = request.AirplaneId,
            Price = request.Price
        };
        await _client.StoreAsync(flight, cancellationToken);
        return flight;
    }
}