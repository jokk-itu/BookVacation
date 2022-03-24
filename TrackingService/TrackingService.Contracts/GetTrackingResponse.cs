namespace TrackingService.Contracts;

#nullable disable
public class GetTrackingResponse
{
    public string Id { get; set; }

    public IEnumerable<StatusDto> Statuses { get; set; }
}