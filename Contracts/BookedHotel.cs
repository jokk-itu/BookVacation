using System;

namespace Contracts
{
    public interface BookedHotel
    {
        public Guid BookHotelId { get; }

        public decimal Price { get; }
    }
}