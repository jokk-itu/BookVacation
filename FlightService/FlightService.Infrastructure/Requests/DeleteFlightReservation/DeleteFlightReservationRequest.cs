using MediatR;

namespace FlightService.Infrastructure.Requests.DeleteFlightReservation;

public record DeleteFlightReservationRequest(string ReservationId) : IRequest<Unit>;