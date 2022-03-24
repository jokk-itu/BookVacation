namespace CarService.Contracts.RentalCar;

#nullable disable
public class PostRentalCarRequest
{
    public Guid CarModelNumber { get; set; }
    public string CarCompanyName { get; set; }
    public string RentingCompanyName { get; set; }
    public decimal DayPrice { get; set; }
    public string Color { get; set; }
}