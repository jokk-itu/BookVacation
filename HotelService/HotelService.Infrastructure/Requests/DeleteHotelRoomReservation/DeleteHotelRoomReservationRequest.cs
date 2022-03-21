using MediatR;

namespace HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;

public record DeleteHotelRoomReservationRequest(string HotelRoomReservationId) : IRequest<Unit>;