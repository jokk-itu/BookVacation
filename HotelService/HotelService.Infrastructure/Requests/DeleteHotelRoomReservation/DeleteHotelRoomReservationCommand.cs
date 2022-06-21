using Mediator;
using MediatR;

namespace HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;

public record DeleteHotelRoomReservationCommand(Guid HotelRoomReservationId) : ICommand<Unit>;