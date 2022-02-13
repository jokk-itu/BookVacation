using MediatR;

namespace FlightService.Requests.CreateReservation;

public record CreateReservationRequest(int SeatId, Guid FlightId, Guid ReservationId) : IRequest<RequestResult>;