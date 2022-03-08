using HotelService.Infrastructure.Requests.DeleteBookHotel;
using MediatR;
using Raven.Client.Documents.Session;

namespace HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;

public class DeleteHotelRoomReservationRequestHandler : IRequestHandler<DeleteHotelRoomReservationRequest, Unit>
{
    private readonly IAsyncDocumentSession _session;

    public DeleteHotelRoomReservationRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<Unit> Handle(DeleteHotelRoomReservationRequest request, CancellationToken cancellationToken)
    {
        _session.Delete(request.HotelRoomReservationId);
        await _session.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}