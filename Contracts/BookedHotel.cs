using System;

namespace Contracts
{
    public interface BookedHotel
    {
        public Guid HotelId { get; }

        public decimal Price { get; }
    }
}