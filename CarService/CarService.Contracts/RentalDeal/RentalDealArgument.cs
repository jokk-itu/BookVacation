namespace CarService.Contracts.RentalDeal;

#nullable disable
public record RentalDealArgument
{
    public DateTimeOffset RentFrom { get; init; }
    public DateTimeOffset RentTo { get; init; }
    public string RentalCarId { get; init; }
}