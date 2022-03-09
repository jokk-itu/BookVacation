using FlightService.Domain;
using MediatR;

namespace FlightService.Infrastructure.Requests.CreateFlight;

public record CreateFlightRequest(DateTimeOffset From, DateTimeOffset To, string FromAirport, string ToAirport,
    string AirplaneId, decimal Price) : IRequest<Flight?>;