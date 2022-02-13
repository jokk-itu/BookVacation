using FlightService.Entities.Nodes;
using MediatR;

namespace FlightService.Requests.CreateFlight;

public record CreateFlightRequest(DateTime From, DateTime To) : IRequest<(RequestResult, Flight?)>;