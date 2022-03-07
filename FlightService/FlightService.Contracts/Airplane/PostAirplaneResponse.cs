namespace FlightService.Contracts.Airplane;

#nullable disable
public class PostAirplaneResponse
{
    public string Id { get; set; }

    public Guid ModelNumber { get; set; }

    public string AirplaneMakerName { get; set; }

    public string AirlineName { get; set; }

    public IEnumerable<SeatDTO> Seats { get; set; }
}