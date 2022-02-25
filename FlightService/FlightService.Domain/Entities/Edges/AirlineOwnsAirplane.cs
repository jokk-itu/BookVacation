using FlightService.Domain.Entities.Nodes;

namespace FlightService.Domain.Entities.Edges;

public class AirlineOwnsAirplane
{
    public Guid Id { get; init; }

    public Airline FromNode { get; init; } = default!;

    public Airplane ToNode { get; init; } = default!;
}