using System;

namespace Contracts
{
    public interface RentCar
    {
        public Guid RentCarId { get; }
        public decimal Price { get; }
    }
}