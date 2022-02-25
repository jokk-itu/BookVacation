using CarService.Domain.Entities.Nodes;

namespace CarService.Domain.Entities.Edges;

public record RentingCompanyOwnsCar
{
    public uint DayPrice { get; init; }

    public RentCar FromNode { get; init; } = default!;

    public RentingCompany ToNode { get; init; } = default!;
}