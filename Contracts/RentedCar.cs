using System;

namespace Contracts
{
    public interface RentedCar
    {
        public Guid RentCarId { get; }

        public decimal Price { get; }
    }
}