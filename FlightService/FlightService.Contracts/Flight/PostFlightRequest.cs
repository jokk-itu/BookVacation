namespace FlightService.Contracts.Flight;

#nullable disable
public class PostFlightRequest
{
    public DateTimeOffset From { get; set; }

    public DateTimeOffset To { get; set; }

    public string FromAirport { get; set; }

    public string ToAirport { get; set; }

    public string AirPlaneId { get; set; }

    public decimal Price { get; set; }
}