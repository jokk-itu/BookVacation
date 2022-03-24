namespace TicketService.Contracts.CreateVacationTickets;

#nullable disable
public class CreateVacationTicketArgument
{
    public Guid FlightId { get; set; }

    public Guid HotelId { get; set; }

    public Guid RoomId { get; set; }

    public Guid CarId { get; set; }

    public string RentingCompanyName { get; set; }
}