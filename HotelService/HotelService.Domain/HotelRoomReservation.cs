namespace HotelService.Domain;

#nullable disable
public class HotelRoomReservation
{
    public string Id { get; init; } = string.Empty;

    public Guid HotelId { get; init; }

    public Guid RoomId { get; init; }

    public DateTimeOffset From { get; init; }

    public DateTimeOffset To { get; init; }
}