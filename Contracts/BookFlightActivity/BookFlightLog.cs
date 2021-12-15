using System;

namespace Contracts.BookFlightActivity;

public record BookFlightLog
{
    public Guid ReservationId { get; init; }
}