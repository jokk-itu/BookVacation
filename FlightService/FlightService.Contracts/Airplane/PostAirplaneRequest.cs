namespace FlightService.Contracts.Airplane;

#nullable disable
public class PostAirplaneRequest
{
    public Guid ModelNumber { get; set; }

    public string AirplaneMakerName { get; set; }

    public string AirlineName { get; set; }

    public short Seats { get; set; }
}