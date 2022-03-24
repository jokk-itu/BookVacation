namespace FlightService.Domain;

#nullable disable
public record Flight
{
    public string Id { get; init; } = string.Empty;

    public DateTimeOffset From { get; init; }

    public DateTimeOffset To { get; init; }

    public string FromAirport { get; init; }

    public string ToAirport { get; init; }

    public Guid AirPlaneId { get; init; }

    public decimal Price { get; init; }
}