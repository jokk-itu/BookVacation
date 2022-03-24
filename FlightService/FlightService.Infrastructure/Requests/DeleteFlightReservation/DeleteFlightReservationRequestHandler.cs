using MediatR;
using Raven.Client.Documents.Session;

namespace FlightService.Infrastructure.Requests.DeleteFlightReservation;

public class DeleteFlightReservationRequestHandler : IRequestHandler<DeleteFlightReservationRequest, Unit>
{
    private readonly IAsyncDocumentSession _session;

    public DeleteFlightReservationRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<Unit> Handle(DeleteFlightReservationRequest request, CancellationToken cancellationToken)
    {
        _session.Delete(request.ReservationId.ToString());
        await _session.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}