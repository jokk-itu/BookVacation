using MediatR;

namespace FlightService.Infrastructure.Requests.DeleteFlightReservation;

public record DeleteFlightReservationRequest(Guid ReservationId) : IRequest<Unit>;