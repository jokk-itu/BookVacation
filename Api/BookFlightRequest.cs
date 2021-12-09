using System;

namespace Api
{
    public class BookFlightRequest
    {
        public Guid FlightId { get; set; }

        public decimal Price { get; set; }
    }
}