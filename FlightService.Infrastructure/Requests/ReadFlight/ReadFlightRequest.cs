using FlightService.Domain.Entities.Nodes;
using MediatR;

namespace FlightService.Infrastructure.Requests.ReadFlight;

public record ReadFlightRequest(Guid Id) : IRequest<(RequestResult, Flight)>;