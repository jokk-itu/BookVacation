using System;

namespace Contracts
{
    public interface CompleteBookFlight
    {
        public Guid FlightId { get; }

        public decimal Price { get; }
    }
}