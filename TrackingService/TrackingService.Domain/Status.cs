namespace TrackingService.Domain;

#nullable disable
public record Status
{
    public string Result { get; init; }

    public DateTimeOffset OccuredAt { get; init; }
}