namespace FlightService.Entities.Nodes;

public class Flight
{
    public Guid Id { get; init; }

    public DateTime From { get; init; }

    public DateTime To { get; init; }
}