using System;

namespace Contracts
{
    public interface CancelBookFlight
    {
        public Guid BookFlightId { get; }

        public decimal Price { get; }
    }
}