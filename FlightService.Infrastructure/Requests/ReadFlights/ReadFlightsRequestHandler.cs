using FlightService.Domain.Entities.Nodes;
using MediatR;
using Neo4j.Driver;

namespace FlightService.Infrastructure.Requests.ReadFlights;

public class ReadFlightsRequestHandler : IRequestHandler<ReadFlightsRequest, (RequestResult, IEnumerable<Flight>)>
{
    private readonly IDriver _driver;

    public ReadFlightsRequestHandler(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<(RequestResult, IEnumerable<Flight>)> Handle(ReadFlightsRequest request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}