namespace CarService.Contracts.RentalCar;

#nullable disable
public record PostRentalCarRequest
{
    public Guid CarModelNumber { get; init; }
    public string CarCompanyName { get; init; }
    public string RentingCompanyName { get; init; }
    public decimal DayPrice { get; init; }
    public string Color { get; init; }
}