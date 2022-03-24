namespace FlightService.Contracts.FlightReservation;

#nullable disable
public class FlightReservationArgument
{
    public Guid FlightId { get; set; }

    public Guid SeatId { get; set; }
}