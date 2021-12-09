using System;

namespace Contracts
{
    public interface ExpireFlight
    {
        Guid FlightId { get; }

        decimal Price { get; }
    }
}