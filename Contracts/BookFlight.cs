using System;

namespace Contracts
{
    public interface BookFlight
    {
        public Guid BookFlightId { get; }
        public decimal Price { get; }
    }
}