namespace TrackingService.Contracts;

#nullable disable
public record GetTrackingResponse
{
    public string Id { get; init; }

    public IEnumerable<StatusDto> Statuses { get; init; }
}