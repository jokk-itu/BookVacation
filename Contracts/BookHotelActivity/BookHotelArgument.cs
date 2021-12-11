using System;

namespace Contracts.BookHotelActivity;

public record BookHotelArgument
{
    public Guid HotelId { get; }
    public decimal Price { get; }
}