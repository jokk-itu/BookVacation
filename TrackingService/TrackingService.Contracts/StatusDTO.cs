namespace TrackingService.Contracts;

#nullable disable
public record StatusDto
{
    public string Id { get; init; }

    public string Result { get; init; }

    public DateTimeOffset OccuredAt { get; init; }
}