using FlightService.Domain;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace FlightService.Infrastructure.Requests.CreateFlightReservation;

public class CreateFlightReservationRequestHandler : IRequestHandler<CreateFlightReservationRequest, FlightReservation?>
{
    private readonly IAsyncDocumentSession _session;

    public CreateFlightReservationRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<FlightReservation?> Handle(CreateFlightReservationRequest request,
        CancellationToken cancellationToken)
    {
        var flight = await _session.Query<Flight>().Where(x => x.Id == request.FlightId)
            .FirstOrDefaultAsync(cancellationToken);

        if (flight is null)
            return null;

        var airplane = await _session.Query<Airplane>()
            .Where(x => x.Id == flight.AirPlaneId && x.Seats.Any(y => y.Id == request.SeatId))
            .FirstOrDefaultAsync(cancellationToken);

        if (airplane is null)
            return null;

        var conflictingReservation = await _session.Query<FlightReservation>()
            .Where(x => x.FlightId == request.FlightId && x.SeatId == request.SeatId)
            .FirstOrDefaultAsync(cancellationToken);

        if (conflictingReservation is not null)
            return null;

        var flightReservation = new FlightReservation
        {
            SeatId = request.SeatId,
            FlightId = request.FlightId
        };

        await _session.StoreAsync(flightReservation, cancellationToken);
        await _session.SaveChangesAsync(cancellationToken);

        return flightReservation;
    }
}