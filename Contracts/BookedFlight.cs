using System;

namespace Contracts
{
    public interface BookedFlight
    {
        public Guid BookFlightId { get; }

        public decimal Price { get; }
    }
}