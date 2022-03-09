namespace TrackingService.Domain;

#nullable disable
public record Tracking
{
    public string Id { get; init; }

    public IEnumerable<Status> Statuses { get; init; }
}