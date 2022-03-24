using MediatR;

namespace HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;

public record DeleteHotelRoomReservationRequest(Guid HotelRoomReservationId) : IRequest<Unit>;