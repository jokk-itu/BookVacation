using DocumentClient;
using HotelService.Domain;
using MediatR;
using Raven.Client.Documents;

namespace HotelService.Infrastructure.Requests.CreateHotelRoomReservation;

public class
    CreateHotelRoomReservationRequestHandler : IRequestHandler<CreateHotelRoomReservationRequest, HotelRoomReservation?>
{
    private readonly IDocumentClient _client;

    public CreateHotelRoomReservationRequestHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<HotelRoomReservation?> Handle(CreateHotelRoomReservationRequest request,
        CancellationToken cancellationToken)
    {
        var hotel = await _client.QueryAsync<Hotel>(async query => await query
            .Where(x => x.Id == request.HotelId.ToString() && x.HotelRooms.Any(y => y.Id == request.RoomId.ToString()))
            .FirstOrDefaultAsync(cancellationToken));

        if (hotel is null)
            return null;

        var conflictingReservation = await _client.QueryAsync<HotelRoomReservation>(async query => await query
            .Where(x =>
                x.HotelId == request.HotelId && x.RoomId == request.RoomId &&
                request.From >= x.From && request.From <= x.To ||
                request.To >= x.From && request.To <= x.To)
            .FirstOrDefaultAsync(cancellationToken));

        if (conflictingReservation is not null)
            return null;

        var hotelRoomReservation = new HotelRoomReservation
        {
            HotelId = request.HotelId,
            RoomId = request.RoomId,
            From = request.From,
            To = request.To
        };

        await _client.StoreAsync(hotelRoomReservation, cancellationToken);
        return hotelRoomReservation;
    }
}