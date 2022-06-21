using FlightService.Domain;
using Mediator;

namespace FlightService.Infrastructure.Requests.CreateAirplane;

public record CreateAirplaneCommand
    (Guid ModelNumber, string AirplaneMakerName, string AirlineName, short Seats) : ICommand<Airplane>;