using FlightService.Domain;
using Mediator;
using MediatR;

namespace FlightService.Infrastructure.Requests.CreateFlightReservation;

public record CreateFlightReservationCommand(Guid SeatId, Guid FlightId) : ICommand<FlightReservation>;