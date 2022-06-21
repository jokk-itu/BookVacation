using DocumentClient;
using FlightService.Domain;
using Mediator;

namespace FlightService.Infrastructure.Requests.CreateAirplane;

public class CreateAirplaneCommandHandler : ICommandHandler<CreateAirplaneCommand, Airplane>
{
    private readonly IDocumentClient _client;

    public CreateAirplaneCommandHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Response<Airplane>> Handle(CreateAirplaneCommand command, CancellationToken cancellationToken)
    {
        var airPlane = new Airplane
        {
            ModelNumber = command.ModelNumber,
            AirplaneMakerName = command.AirplaneMakerName,
            AirlineName = command.AirlineName,
            Seats = Enumerable.Range(0, command.Seats).Select(_ => new Seat { Id = Guid.NewGuid().ToString() }).ToList()
        };
        await _client.StoreAsync(airPlane, cancellationToken);
        return new Response<Airplane>(airPlane);
    }
}