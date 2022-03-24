using HotelService.Domain;
using MediatR;

namespace HotelService.Infrastructure.Requests.CreateHotelRoomReservation;

public record CreateHotelRoomReservationRequest
    (Guid HotelId, Guid RoomId, DateTimeOffset From, DateTimeOffset To) : IRequest<HotelRoomReservation?>;