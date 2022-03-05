namespace CarService.Domain;

public record RentalCar
{
    public string Id { get; init; } = string.Empty;

    public Guid CarModelNumber { get; init; }

    public string CarCompanyName { get; init; } = string.Empty;

    public string RentalCompanyName { get; init; } = string.Empty;

    public decimal DayPrice { get; init; }

    public string Color { get; init; } = string.Empty;
}