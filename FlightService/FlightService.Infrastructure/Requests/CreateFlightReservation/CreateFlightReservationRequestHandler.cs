using DocumentClient;
using FlightService.Domain;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace FlightService.Infrastructure.Requests.CreateFlightReservation;

public class CreateFlightReservationRequestHandler : IRequestHandler<CreateFlightReservationRequest, FlightReservation?>
{
    private readonly IDocumentClient _client;

    public CreateFlightReservationRequestHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<FlightReservation?> Handle(CreateFlightReservationRequest request,
        CancellationToken cancellationToken)
    {
        var flight = await _client.QueryAsync<Flight>(query => query.Where(x => x.Id == request.FlightId.ToString())
            .FirstOrDefaultAsync(cancellationToken));

        if (flight is null)
            return null;

        var airplane = await _client.QueryAsync<Airplane>(query => query
            .Where(x => x.Id == flight.AirPlaneId.ToString() && x.Seats.Any(y => y.Id == request.SeatId.ToString()))
            .FirstOrDefaultAsync(cancellationToken));

        if (airplane is null)
            return null;

        var conflictingReservation = await _client.QueryAsync<FlightReservation>(query => query
            .Where(x => x.FlightId == request.FlightId && x.SeatId == request.SeatId)
            .FirstOrDefaultAsync(cancellationToken));

        if (conflictingReservation is not null)
            return null;

        var flightReservation = new FlightReservation
        {
            SeatId = request.SeatId,
            FlightId = request.FlightId
        };
        await _client.StoreAsync(flightReservation, cancellationToken);
        return flightReservation;
    }
}