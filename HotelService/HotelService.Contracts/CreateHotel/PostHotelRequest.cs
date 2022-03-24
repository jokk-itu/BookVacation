namespace HotelService.Contracts.CreateHotel;

#nullable disable
public class PostHotelRequest
{
    public short Rooms { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    public string Address { get; set; }
}