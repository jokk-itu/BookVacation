namespace CarService.Domain.Entities.Nodes;

public record RentingCompany
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;
}