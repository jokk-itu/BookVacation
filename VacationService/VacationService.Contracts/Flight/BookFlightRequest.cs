namespace VacationService.Contracts.Flight;

public record BookFlightRequest
{
    public Guid FlightId { get; init; }

    public decimal Price { get; init; }
}