namespace CarService.Contracts.BookHotelActivity;

public record BookHotelArgument
{
    public Guid HotelId { get; init; }
    public uint Days { get; init; }

    public Guid RoomId { get; init; }
}