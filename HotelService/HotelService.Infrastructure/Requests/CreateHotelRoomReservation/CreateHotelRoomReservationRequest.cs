using HotelService.Domain;
using MediatR;

namespace HotelService.Infrastructure.Requests.CreateBookHotel;

public record CreateHotelRoomReservationRequest(string HotelId, string RoomId, DateTimeOffset From, DateTimeOffset To) : IRequest<HotelRoomReservation?>;