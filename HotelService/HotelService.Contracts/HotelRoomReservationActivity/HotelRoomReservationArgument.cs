namespace HotelService.Contracts.HotelRoomReservationActivity;

#nullable disable
public record HotelRoomReservationArgument
{
    public string HotelId { get; init; }

    public string RoomId { get; init; }

    public DateTimeOffset From { get; init; }

    public DateTimeOffset To { get; set; }
}