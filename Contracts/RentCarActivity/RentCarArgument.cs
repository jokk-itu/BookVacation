using System;

namespace Contracts.RentCarActivity;

public record RentCarArgument
{
    public Guid CarId { get; init; }
    public decimal Price { get; init; }
}