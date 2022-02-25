namespace FlightService.Contracts.GetFlight;

public record GetFlightResponse
{
    public Guid Id { get; init; }

    public DateTime From { get; init; }

    public DateTime To { get; init; }
}