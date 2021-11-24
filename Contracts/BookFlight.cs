using System;

namespace Contracts
{
    public interface BookFlight
    {
        public Guid FlightId { get; }
        public decimal Price { get; }
    }
}