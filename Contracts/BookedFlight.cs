using System;

namespace Contracts
{
    public interface BookedFlight
    {
        public Guid FlightId { get; }

        public decimal Price { get; }
    }
}