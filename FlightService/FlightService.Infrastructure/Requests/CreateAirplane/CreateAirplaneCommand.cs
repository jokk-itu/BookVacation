using FlightService.Domain;
using Mediator;
using MediatR;

namespace FlightService.Infrastructure.Requests.CreateAirplane;

public record CreateAirplaneCommand
    (Guid ModelNumber, string AirplaneMakerName, string AirlineName, short Seats) : ICommand<Airplane>;