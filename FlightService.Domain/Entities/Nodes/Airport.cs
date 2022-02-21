namespace FlightService.Domain.Entities.Nodes;

public record Airport
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;
}