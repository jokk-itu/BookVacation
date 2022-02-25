namespace FlightService.Domain.Entities.Nodes;

public record Seat
{
    public int Id { get; init; }

    public string Class { get; init; } = string.Empty;
}