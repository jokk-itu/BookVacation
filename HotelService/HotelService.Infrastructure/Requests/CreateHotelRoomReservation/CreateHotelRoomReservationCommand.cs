using HotelService.Domain;
using Mediator;

namespace HotelService.Infrastructure.Requests.CreateHotelRoomReservation;

public record CreateHotelRoomReservationCommand
    (Guid HotelId, Guid RoomId, DateTimeOffset From, DateTimeOffset To) : ICommand<HotelRoomReservation>;