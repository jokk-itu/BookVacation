using FlightService.Contracts.DTO;

namespace FlightService.Contracts.GetFlights;

public class GetFlightsResponse
{
    public IEnumerable<Flight> Flights { get; init; } = new List<Flight>();
}