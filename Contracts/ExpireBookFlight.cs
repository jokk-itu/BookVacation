using System;

namespace Contracts
{
    public interface ExpireBookFlight
    {
        Guid BookFlightId { get; }
        
        decimal Price { get; }
    }
}