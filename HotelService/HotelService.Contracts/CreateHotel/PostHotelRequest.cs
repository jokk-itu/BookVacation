namespace HotelService.Contracts.CreateHotel;

#nullable disable
public record PostHotelRequest
{
    public short Rooms { get; init; }

    public string Country { get; init; }

    public string City { get; init; }

    public string Address { get; init; }
}