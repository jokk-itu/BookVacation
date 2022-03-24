using FlightService.Domain;
using MediatR;

namespace FlightService.Infrastructure.Requests.CreateFlightReservation;

public record CreateFlightReservationRequest(Guid SeatId, Guid FlightId) : IRequest<FlightReservation?>;