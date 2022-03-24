namespace TrackingService.Domain;

#nullable disable
public record Status
{
    public string Name { get; init; }

    public string Result { get; init; }

    public DateTimeOffset OccuredAt { get; init; }
}