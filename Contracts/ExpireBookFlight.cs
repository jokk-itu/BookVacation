using System;

namespace Contracts
{
    public interface ExpireBookFlight
    {
        Guid FlightId { get; }

        decimal Price { get; }
    }
}