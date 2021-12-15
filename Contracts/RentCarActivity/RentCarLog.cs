using System;

namespace Contracts.RentCarActivity;

public record RentCarLog
{
    public Guid RentCarId { get; init; }
}