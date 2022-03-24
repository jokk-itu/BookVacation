namespace HotelService.Domain;

#nullable disable
public record Hotel
{
    public string Id { get; init; } = string.Empty;

    public ICollection<HotelRoom> HotelRooms { get; init; }

    public string Country { get; init; }

    public string City { get; init; }

    public string Address { get; init; }
}