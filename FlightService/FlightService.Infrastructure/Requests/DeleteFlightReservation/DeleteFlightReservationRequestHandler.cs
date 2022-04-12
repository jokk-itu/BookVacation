using DocumentClient;
using MediatR;

namespace FlightService.Infrastructure.Requests.DeleteFlightReservation;

public class DeleteFlightReservationRequestHandler : IRequestHandler<DeleteFlightReservationRequest, Unit>
{
    private readonly IDocumentClient _client;

    public DeleteFlightReservationRequestHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Unit> Handle(DeleteFlightReservationRequest request, CancellationToken cancellationToken)
    {
        await _client.DeleteAsync(request.ReservationId.ToString(), cancellationToken);
        return Unit.Value;
    }
}