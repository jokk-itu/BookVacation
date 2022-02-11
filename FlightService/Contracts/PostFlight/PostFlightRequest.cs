namespace FlightService.Contracts.PostFlight;

public record PostFlightRequest
{
    public DateTime From { get; init; }

    public DateTime To { get; init; }
}