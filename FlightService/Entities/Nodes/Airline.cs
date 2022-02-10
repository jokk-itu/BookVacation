namespace FlightService.Entities.Nodes;

public record Airline
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;
}