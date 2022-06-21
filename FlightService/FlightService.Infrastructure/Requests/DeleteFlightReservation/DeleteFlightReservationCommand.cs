using Mediator;
using MediatR;

namespace FlightService.Infrastructure.Requests.DeleteFlightReservation;

public record DeleteFlightReservationCommand(Guid ReservationId) : ICommand<Unit>;