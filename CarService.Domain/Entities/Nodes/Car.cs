namespace CarService.Domain.Entities.Nodes;

public record Car
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public uint Seat { get; init; }

    public string Color { get; init; } = string.Empty;

    public uint Year { get; init; }
}