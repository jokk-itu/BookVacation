using FlightService.Domain;
using MediatR;
using Raven.Client.Documents.Session;

namespace FlightService.Infrastructure.Requests.CreateAirplane;

public class CreateAirplaneRequestHandler : IRequestHandler<CreateAirplaneRequest, Airplane>
{
    private readonly IAsyncDocumentSession _session;

    public CreateAirplaneRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<Airplane> Handle(CreateAirplaneRequest request, CancellationToken cancellationToken)
    {
        var airPlane = new Airplane
        {
            ModelNumber = request.ModelNumber,
            AirplaneMakerName = request.AirplaneMakerName,
            AirlineName = request.AirlineName,
            Seats = Enumerable.Range(0, request.Seats).Select(_ => new Seat {Id = Guid.NewGuid().ToString()}).ToList()
        };
        await _session.StoreAsync(airPlane, cancellationToken);
        await _session.SaveChangesAsync(cancellationToken);
        return airPlane;
    }
}