using System;

namespace Contracts
{
    public interface CompleteBookFlight
    {
        public Guid BookFlightId { get; }

        public decimal Price { get; }
    }
}