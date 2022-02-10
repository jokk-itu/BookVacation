using CarService.Entities.Nodes;

namespace CarService.Entities.Edges;

public record RentingCompanyOwnsCar
{
    public uint DayPrice { get; init; }

    public RentCar FromNode { get; init; } = default!;

    public RentingCompany ToNode { get; init; } = default!;
}