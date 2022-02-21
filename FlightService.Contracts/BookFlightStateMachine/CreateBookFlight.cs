namespace FlightService.Contracts.BookFlightStateMachine;

public record CreateBookFlight
{
    public Guid FlightId { get; }

    public decimal Price { get; }
}