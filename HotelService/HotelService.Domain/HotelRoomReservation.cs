namespace HotelService.Domain;

#nullable disable
public class HotelRoomReservation
{
    public string Id { get; init; } = string.Empty;

    public string HotelId { get; init; }

    public string RoomId { get; init; }

    public DateTimeOffset From { get; init; }
    
    public DateTimeOffset To { get; init; }
}