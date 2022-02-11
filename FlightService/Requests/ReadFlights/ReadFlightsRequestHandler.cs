using FlightService.Entities.Nodes;
using MediatR;
using Neo4j.Driver;

namespace FlightService.Requests.ReadFlights;

public class ReadFlightsRequestHandler : IRequestHandler<ReadFlightsRequest, (Response, IEnumerable<Flight>)>
{
    private readonly IDriver _driver;

    public ReadFlightsRequestHandler(IDriver driver)
    {
        _driver = driver;
    }
    
    public async Task<(Response, IEnumerable<Flight>)> Handle(ReadFlightsRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}