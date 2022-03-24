namespace HotelService.Contracts.CreateHotel;

#nullable disable
public class PostHotelResponse
{
    public Guid Id { get; set; }

    public IEnumerable<HotelRoomDTO> HotelRooms { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    public string Address { get; set; }
}