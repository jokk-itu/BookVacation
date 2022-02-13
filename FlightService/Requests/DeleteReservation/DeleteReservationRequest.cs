using MediatR;

namespace FlightService.Requests.DeleteReservation;

public record DeleteReservationRequest(Guid ReservationId) : IRequest<RequestResult>;