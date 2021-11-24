namespace Contracts
{
    public interface BookVacation
    {
        public BookFlight Flight { get; }

        public BookHotel Hotel { get; }

        public RentCar RentCar { get; }
    }
}