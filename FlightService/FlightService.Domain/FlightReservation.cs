namespace FlightService.Domain;

#nullable disable
public class FlightReservation
{
    public string Id { get; init; } = string.Empty;

    public Guid FlightId { get; init; }

    public Guid SeatId { get; init; }
}