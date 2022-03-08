using HotelService.Domain;
using MediatR;
using Raven.Client.Documents.Session;

namespace HotelService.Infrastructure.Requests.CreateBookHotel;

public class CreateHotelRoomReservationRequestHandler : IRequestHandler<CreateHotelRoomReservationRequest, HotelRoomReservation?>
{
    private readonly IAsyncDocumentSession _session;

    public CreateHotelRoomReservationRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<HotelRoomReservation?> Handle(CreateHotelRoomReservationRequest request, CancellationToken cancellationToken)
    {
        //Check if the Hotel exists
        
        //Check if the Room exists
        
        //Check for conflicting Reservations
        throw new NotImplementedException();
    }
}