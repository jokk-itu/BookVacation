using MediatR;

namespace FlightService.Infrastructure.Requests.DeleteBookFlight;

public record DeleteBookFlightRequest(Guid ReservationId) : IRequest<RequestResult>;