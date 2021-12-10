using System;

namespace Contracts.BookFlightStateMachine
{
    public record CompleteBookFlight
    {
        public Guid FlightId { get; }

        public decimal Price { get; }
    }
}