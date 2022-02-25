using FlightService.Domain.Entities.Nodes;
using MediatR;

namespace FlightService.Infrastructure.Requests.CreateFlight;

public record CreateFlightRequest(DateTime From, DateTime To) : IRequest<(RequestResult, Flight?)>;