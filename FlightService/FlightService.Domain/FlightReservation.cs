namespace FlightService.Domain;

#nullable disable
public class FlightReservation
{
    public string Id { get; init; } = string.Empty;

    public string FlightId { get; init; }

    public string SeatId { get; init; }
}