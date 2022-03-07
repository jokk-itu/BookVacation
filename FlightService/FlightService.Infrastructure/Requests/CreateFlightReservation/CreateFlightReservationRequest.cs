using FlightService.Domain;
using MediatR;

namespace FlightService.Infrastructure.Requests.CreateFlightReservation;

public record CreateFlightReservationRequest(string SeatId, string FlightId) : IRequest<FlightReservation?>;