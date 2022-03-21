using FlightService.Domain;
using MediatR;

namespace FlightService.Infrastructure.Requests.CreateAirplane;

public record CreateAirplaneRequest
    (Guid ModelNumber, string AirplaneMakerName, string AirlineName, short Seats) : IRequest<Airplane>;