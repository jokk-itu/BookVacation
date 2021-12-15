using System;

namespace Contracts.BookFlightActivity;

public record BookFlightArgument
{
    public Guid FlightId { get; init; }

    public int SeatId { get; init; }
}