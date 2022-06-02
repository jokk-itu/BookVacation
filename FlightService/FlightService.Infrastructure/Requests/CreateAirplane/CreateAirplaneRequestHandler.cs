using DocumentClient;
using FlightService.Domain;
using MediatR;

namespace FlightService.Infrastructure.Requests.CreateAirplane;

public class CreateAirplaneRequestHandler : IRequestHandler<CreateAirplaneRequest, Airplane>
{
    private readonly IDocumentClient _client;

    public CreateAirplaneRequestHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Airplane> Handle(CreateAirplaneRequest request, CancellationToken cancellationToken)
    {
        var airPlane = new Airplane
        {
            ModelNumber = request.ModelNumber,
            AirplaneMakerName = request.AirplaneMakerName,
            AirlineName = request.AirlineName,
            Seats = Enumerable.Range(0, request.Seats).Select(_ => new Seat { Id = Guid.NewGuid().ToString() }).ToList()
        };
        await _client.StoreAsync(airPlane, cancellationToken);
        return airPlane;
    }
}