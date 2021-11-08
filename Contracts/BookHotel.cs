using System;

namespace Contracts
{
    public interface BookHotel
    {
        public Guid BookHotelId { get; }
        public decimal Price { get; }
    }
}