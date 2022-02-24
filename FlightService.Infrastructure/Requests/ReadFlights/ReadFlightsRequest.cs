using FlightService.Domain.Entities.Nodes;
using MediatR;

namespace FlightService.Infrastructure.Requests.ReadFlights;

public record ReadFlightsRequest(uint Amount, uint Offset) : IRequest<(RequestResult, IEnumerable<Flight>?)>;