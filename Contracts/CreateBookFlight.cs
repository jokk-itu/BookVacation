using System;

namespace Contracts
{
    public interface CreateBookFlight
    {
        public Guid BookFlightId { get; }

        public decimal Price { get; }
    }
}