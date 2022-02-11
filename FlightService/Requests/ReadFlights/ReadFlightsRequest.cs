using FlightService.Entities.Nodes;
using MediatR;

namespace FlightService.Requests.ReadFlights;

public record ReadFlightsRequest(uint Amount, uint Offset) : IRequest<(Response, IEnumerable<Flight>)>;