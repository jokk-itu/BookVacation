namespace HotelService.Contracts.HotelRoomReservationActivity;

#nullable disable
public class HotelRoomReservationArgument
{
    public Guid HotelId { get; set; }

    public Guid RoomId { get; set; }

    public DateTimeOffset From { get; set; }

    public DateTimeOffset To { get; set; }
}