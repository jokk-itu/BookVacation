using System;

namespace Contracts.RentCarActivity;

public record RentCarArgument
{
    public Guid CarId { get; init; }
    public Guid RentingCompanyId { get; init; }
    public uint Days { get; init; }
}