namespace FlightService.Domain.Entities.Nodes;

public class Airplane
{
    public Guid Id { get; init; }

    public uint Seats { get; init; }

    public string Name { get; init; } = string.Empty;
}