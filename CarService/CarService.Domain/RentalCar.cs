namespace CarService.Domain;

#nullable disable
public record RentalCar
{
    public string Id { get; init; } = string.Empty;

    public Guid CarModelNumber { get; init; }

    public string CarCompanyName { get; init; }

    public string RentalCompanyName { get; init; }

    public decimal DayPrice { get; init; }

    public string Color { get; init; }
}