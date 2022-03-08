using MediatR;

namespace HotelService.Infrastructure.Requests.DeleteBookHotel;

public record DeleteHotelRoomReservationRequest(string HotelRoomReservationId) : IRequest<Unit>;