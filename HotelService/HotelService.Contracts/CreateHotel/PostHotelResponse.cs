namespace HotelService.Contracts.CreateHotel;

#nullable disable
public class PostHotelResponse
{
    public string Id { get; init; }

    public IEnumerable<HotelRoomDTO> HotelRooms { get; init; }

    public string Country { get; init; }

    public string City { get; init; }

    public string Address { get; init; }
}