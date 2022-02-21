namespace FlightService.Contracts.BookFlightStateMachine;

public record CancelBookFlight
{
    public Guid FlightId { get; }

    public decimal Price { get; }
}