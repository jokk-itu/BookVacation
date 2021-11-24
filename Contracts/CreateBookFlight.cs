using System;

namespace Contracts
{
    public interface CreateBookFlight
    {
        public Guid FlightId { get; }

        public decimal Price { get; }
    }
}