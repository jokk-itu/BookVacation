using DocumentClient;
using MediatR;

namespace HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;

public class DeleteHotelRoomReservationRequestHandler : IRequestHandler<DeleteHotelRoomReservationRequest, Unit>
{
    private readonly IDocumentClient _client;

    public DeleteHotelRoomReservationRequestHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Unit> Handle(DeleteHotelRoomReservationRequest request, CancellationToken cancellationToken)
    {
        await _client.DeleteAsync(request.HotelRoomReservationId.ToString(), cancellationToken);
        return Unit.Value;
    }
}