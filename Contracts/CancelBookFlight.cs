using System;

namespace Contracts
{
    public interface CancelBookFlight
    {
        public Guid FlightId { get; }

        public decimal Price { get; }
    }
}