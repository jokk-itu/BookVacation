using System;

namespace Api
{
    public record VacationRequest
    {
        public Guid FlightId { get; init; }
        public decimal FlightPrice { get; init; }

        public Guid HotelId { get; init; }
        public decimal HotelPrice { get; init; }

        public Guid RentCarId { get; init; }
        public decimal CarPrice { get; init; }
    }
}