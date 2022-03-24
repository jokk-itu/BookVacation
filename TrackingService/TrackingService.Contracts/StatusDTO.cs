namespace TrackingService.Contracts;

#nullable disable
public class StatusDto
{
    public string Result { get; set; }

    public DateTimeOffset OccuredAt { get; set; }
}