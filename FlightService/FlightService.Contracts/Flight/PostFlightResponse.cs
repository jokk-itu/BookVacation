namespace FlightService.Contracts.Flight;

#nullable disable
public class PostFlightResponse
{
    public Guid Id { get; set; }

    public DateTimeOffset From { get; set; }

    public DateTimeOffset To { get; set; }

    public string FromAirport { get; set; }

    public string ToAirport { get; set; }

    public Guid AirPlaneId { get; set; }

    public decimal Price { get; set; }
}