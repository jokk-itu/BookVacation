namespace CarService.Entities.Nodes;

public record CarCompany
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;
}