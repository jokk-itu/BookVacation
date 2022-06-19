using FlightService.Domain;
using Mediator;
using MediatR;

namespace FlightService.Infrastructure.Requests.CreateFlight;

public record CreateFlightCommand(DateTimeOffset From, DateTimeOffset To, string FromAirport, string ToAirport,
    Guid AirplaneId, decimal Price) : ICommand<Flight>;