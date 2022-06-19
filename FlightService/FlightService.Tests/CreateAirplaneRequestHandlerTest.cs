using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Domain;
using FlightService.Infrastructure.Requests.CreateAirplane;
using Microsoft.Extensions.Logging;
using Moq;
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
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var request = new CreateAirplaneCommand(Guid.NewGuid(), "Boeing", "SAS", 20);
        var handler = new CreateAirplaneCommandHandler(client);

        //Act
        var airplaneResponse = await handler.Handle(request, CancellationToken.None);
        WaitForIndexing(store);
        var actual = await session.Query<Airplane>().Where(x => x.Id == airplaneResponse.Body!.Id).FirstOrDefaultAsync();

        //Assert
        Assert.NotNull(actual);
    }
}