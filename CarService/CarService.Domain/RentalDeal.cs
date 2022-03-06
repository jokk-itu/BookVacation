namespace CarService.Domain;

#nullable disable
public record RentalDeal
{
    public string Id { get; init; }
    
    public DateTimeOffset RentFrom { get; init; }

    public DateTimeOffset RentTo { get; init; }

    public string RentalCarId { get; init; }
}