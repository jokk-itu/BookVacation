namespace Contracts
{
    public interface BookVacation
    {
        public BookFlight BookFlight { get; }

        public BookHotel BookHotel { get; set; }

        public RentCar RentCar { get; set; }
    }
}