namespace TrackingService.Domain;

#nullable disable
public record Tracking
{
    public string Id { get; init; }

    public ICollection<Status> Statuses { get; init; }
}