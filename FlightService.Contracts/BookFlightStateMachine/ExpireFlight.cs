namespace FlightService.Contracts.BookFlightStateMachine;

public record ExpireFlight
{
    public Guid FlightId { get; }

    public decimal Price { get; }
}