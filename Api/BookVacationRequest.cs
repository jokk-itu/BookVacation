using System;

namespace Api
{
    public record BookVacationRequest
    {
        public Guid BookFlightId { get; init; }
        public decimal FlightPrice { get; init; }

        public Guid BookHotelId { get; init; }
        public decimal HotelPrice { get; init; }

        public Guid RentCarId { get; init; }
        public decimal CarPrice { get; init; }
    }
}