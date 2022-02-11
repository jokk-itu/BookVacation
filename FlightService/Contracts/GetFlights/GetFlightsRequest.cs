using Microsoft.AspNetCore.Mvc;

namespace FlightService.Contracts.GetFlights;

public record GetFlightsRequest
{
    [FromQuery]
    public uint Amount { get; init; }

    [FromQuery]
    public uint Offset { get; set; }
}