namespace VacationService.Contracts.Vacation;

#nullable disable
public class PostVacationRequest
{
    public Guid FlightId { get; set; }
    public Guid FlightSeatId { get; set; }
    public Guid HotelId { get; set; }
    public DateTimeOffset HotelFrom { get; set; }
    public DateTimeOffset HotelTo { get; set; }
    public Guid HotelRoomId { get; set; }
    public Guid RentalCarId { get; set; }
    public string RentingCompanyName { get; set; }
    public DateTimeOffset RentalCarFrom { get; set; }
    public DateTimeOffset RentalCarTo { get; set; }
}