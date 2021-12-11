using System;

namespace Api
{
    public record BookFlightRequest
    {
        public Guid FlightId { get; init; }

        public decimal Price { get; init; }
    }
}