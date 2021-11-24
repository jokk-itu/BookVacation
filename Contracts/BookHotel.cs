using System;

namespace Contracts
{
    public interface BookHotel
    {
        public Guid HotelId { get; }
        public decimal Price { get; }
    }
}