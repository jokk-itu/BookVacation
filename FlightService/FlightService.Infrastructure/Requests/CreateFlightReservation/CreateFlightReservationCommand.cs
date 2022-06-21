using FlightService.Domain;
using Mediator;

namespace FlightService.Infrastructure.Requests.CreateFlightReservation;

public record CreateFlightReservationCommand(Guid SeatId, Guid FlightId) : ICommand<FlightReservation>;