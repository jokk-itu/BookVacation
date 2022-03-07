namespace FlightService.Contracts.FlightReservation;

#nullable disable
public record FlightReservationArgument
{
    public string FlightId { get; init; }

    public string SeatId { get; init; }
}