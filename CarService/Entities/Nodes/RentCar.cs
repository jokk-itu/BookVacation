namespace CarService.Entities.Nodes;

public record RentCar
{
    public Guid Id { get; init; }

    public uint Days { get; init; }
}