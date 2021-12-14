using System;

namespace Contracts.BookHotelActivity;

public record BookHotelLog
{
    public Guid RentId { get; set; }
}