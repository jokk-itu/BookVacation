using FlightService.Domain;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace FlightService.Infrastructure.Requests.CreateFlight;

public class CreateFlightRequestHandler : IRequestHandler<CreateFlightRequest, Flight?>
{
    private readonly IAsyncDocumentSession _session;

    public CreateFlightRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }
    
    public async Task<Flight?> Handle(CreateFlightRequest request, CancellationToken cancellationToken)
    {
        var airplane = await _session.Query<Airplane>().Where(x => x.Id == request.AirplaneId)
            .FirstOrDefaultAsync(cancellationToken);

        if (airplane is null)
            return null;
        
        var conflictingFlight = await _session.Query<Flight>()
            .Where(x => x.AirPlaneId == request.AirplaneId)
            .Where(x => request.From >= x.From && request.From <= x.To || 
                        request.To >= x.From && request.To <= x.To)
            .FirstOrDefaultAsync(cancellationToken);

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
        await _session.StoreAsync(flight, cancellationToken);
        await _session.SaveChangesAsync(cancellationToken);
        return flight;
    }
};