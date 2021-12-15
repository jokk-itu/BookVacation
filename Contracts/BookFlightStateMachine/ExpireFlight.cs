using System;

namespace Contracts.BookFlightStateMachine;

public record ExpireFlight
{
    public Guid FlightId { get; }

    public decimal Price { get; }
}