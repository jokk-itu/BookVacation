using MediatR;

namespace FlightService.Infrastructure.Requests.CreateBookFlight;

public record CreateBookFlightRequest(int SeatId, Guid FlightId, Guid ReservationId) : IRequest<RequestResult>;