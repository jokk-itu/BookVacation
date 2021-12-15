using System;

namespace Contracts.BookFlightStateMachine;

public record CancelBookFlight
{
    public Guid FlightId { get; }

    public decimal Price { get; }
}