namespace CarService.Domain;

public record RentalDeal
{
    public string Id { get; init; } = string.Empty;
    
    public DateTimeOffset RentFrom { get; init; }

    public DateTimeOffset RentTo { get; init; }

    public Guid RentalCarId { get; init; }
}