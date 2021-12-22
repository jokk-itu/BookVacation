using System;

namespace Contracts.BookHotelActivity;

public record BookHotelArgument
{
    public Guid HotelId { get; init; }
    public int Days { get; init; }

    public Guid RoomId { get; init; }
}