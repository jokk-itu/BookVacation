using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Domain;
using FlightService.Infrastructure.Requests.CreateAirplane;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace FlightService.Tests;

public class CreateAirplaneRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectFlight()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var request = new CreateAirplaneRequest(Guid.NewGuid(), "Boeing", "SAS", 20);
        var handler = new CreateAirplaneRequestHandler(session);

        //Act
        var expect = await handler.Handle(request, CancellationToken.None);
        WaitForIndexing(store);
        var actual = await session.Query<Airplane>().Where(x => x.Id == expect.Id).FirstOrDefaultAsync();

        //Assert
        Assert.NotNull(actual);
    }
}