namespace CarService.Contracts.RentalDeal;

#nullable disable
public class RentalDealArgument
{
    public DateTimeOffset RentFrom { get; set; }
    public DateTimeOffset RentTo { get; set; }
    public Guid RentalCarId { get; set; }
}