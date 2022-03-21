namespace TrackingService.Domain;

#nullable disable
public record Status
{
    public string Id { get; init; } = string.Empty;

    public string Result { get; init; }

    public DateTimeOffset OccuredAt { get; init; }
}