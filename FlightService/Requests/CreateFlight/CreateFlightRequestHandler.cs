
using FlightService.Entities.Nodes;
using MediatR;
using Neo4j.Driver;

namespace FlightService.Requests.CreateFlight;

public class CreateFlightRequestHandler : IRequestHandler<CreateFlightRequest, (Response, Flight)>
{
    private readonly IDriver _driver;

    public CreateFlightRequestHandler(IDriver driver)
    {
        _driver = driver;
    }
    
    public async Task<(Response, Flight)> Handle(CreateFlightRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}