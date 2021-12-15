using System;

namespace Contracts.BookFlightStateMachine;

public record CreateBookFlight
{
    public Guid FlightId { get; }

    public decimal Price { get; }
}