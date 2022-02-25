namespace FlightService.Contracts.GetFlights;

public record GetFlightsRequest
{
    public uint Amount { get; init; }
    public uint Offset { get; set; }
}