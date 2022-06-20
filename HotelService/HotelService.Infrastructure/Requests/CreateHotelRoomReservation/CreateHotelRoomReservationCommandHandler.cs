using DocumentClient;
using HotelService.Domain;
using Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;

namespace HotelService.Infrastructure.Requests.CreateHotelRoomReservation;

public class
    CreateHotelRoomReservationCommandHandler : ICommandHandler<CreateHotelRoomReservationCommand, HotelRoomReservation>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<CreateHotelRoomReservationCommandHandler> _logger;

    public CreateHotelRoomReservationCommandHandler(IDocumentClient client,
        ILogger<CreateHotelRoomReservationCommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<HotelRoomReservation>> Handle(CreateHotelRoomReservationCommand command,
        CancellationToken cancellationToken)
    {
        var hotel = await _client.QueryAsync<Hotel>(async query => await query
            .Where(x => x.Id == command.HotelId.ToString() && x.HotelRooms.Any(y => y.Id == command.RoomId.ToString()))
            .FirstOrDefaultAsync(cancellationToken));

        if (hotel is null)
        {
            _logger.LogDebug("Hotel with identifier {Identifier} or room {} does not exist", command.HotelId,
                command.RoomId);
            return new Response<HotelRoomReservation>(ResponseCode.NotFound, new []
            {
                "Hotel or Room does not exist"
            });
        }

        var conflictingReservation = await _client.QueryAsync<HotelRoomReservation>(async query => await query
            .Where(x =>
                (x.HotelId == command.HotelId && x.RoomId == command.RoomId &&
                 command.From >= x.From && command.From <= x.To) ||
                (command.To >= x.From && command.To <= x.To))
            .FirstOrDefaultAsync(cancellationToken));

        if (conflictingReservation is not null)
        {
            _logger.LogDebug("Room {} is already booked", command.RoomId);
            return new Response<HotelRoomReservation>(ResponseCode.Conflict, new []
            {
                "Room is already booked"
            });
        }

        var hotelRoomReservation = new HotelRoomReservation
        {
            HotelId = command.HotelId,
            RoomId = command.RoomId,
            From = command.From,
            To = command.To
        };

        await _client.StoreAsync(hotelRoomReservation, cancellationToken);
        return new Response<HotelRoomReservation>(hotelRoomReservation);
    }
}