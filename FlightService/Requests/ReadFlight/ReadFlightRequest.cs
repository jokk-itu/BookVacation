using FlightService.Entities.Nodes;
using MediatR;

namespace FlightService.Requests.ReadFlight;

public record ReadFlightRequest(Guid Id) : IRequest<(RequestResult, Flight)>;