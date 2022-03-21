namespace FlightService.Domain;

#nullable disable
public record Airplane
{
    public string Id { get; init; } = string.Empty;

    public Guid ModelNumber { get; init; }

    public string AirplaneMakerName { get; init; }

    public string AirlineName { get; init; }

    public IEnumerable<Seat> Seats { get; init; }
}