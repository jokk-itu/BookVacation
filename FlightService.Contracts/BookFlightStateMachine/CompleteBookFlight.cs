namespace FlightService.Contracts.BookFlightStateMachine;

public record CompleteBookFlight
{
    public Guid FlightId { get; }

    public decimal Price { get; }
}