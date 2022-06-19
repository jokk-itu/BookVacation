using System;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Domain;
using FlightService.Infrastructure.Requests.CreateAirplane;
using FlightService.Infrastructure.Requests.CreateFlight;
using Mediator;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents.Linq;
using Raven.TestDriver;
using Xunit;

namespace FlightService.Tests;

public class CreateFlightRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectFlight()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createAirplaneRequest = new CreateAirplaneCommand(Guid.NewGuid(), "Boeing", "SAS", 20);
        var createAirplaneHandler = new CreateAirplaneCommandHandler(client);
        var airplaneResponse = await createAirplaneHandler.Handle(createAirplaneRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createFlightRequest = new CreateFlightCommand(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2),
            "Kastrup", "Karup", Guid.Parse(airplaneResponse.Body!.Id), 1200);
        var createFlightHandler = new CreateFlightCommandHandler(client, Mock.Of<ILogger<CreateFlightCommandHandler>>());

        //Act
        var flightResponse = await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);
        WaitForIndexing(store);
        var actual = session.Query<Flight>().Where(x => x.Id == flightResponse.Body!.Id);

        //Assert
        Assert.NotNull(actual);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_InvalidAirplaneId_ExpectNotFound()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createFlightRequest = new CreateFlightCommand(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2),
            "Kastrup", "Karup", Guid.NewGuid(), 1200);
        var createFlightHandler = new CreateFlightCommandHandler(client, Mock.Of<ILogger<CreateFlightCommandHandler>>());

        //Act
        var flightResponse = await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.NotFound, flightResponse.ResponseCode);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_AddIdenticalFlight_ExpectConflict()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createAirplaneRequest = new CreateAirplaneCommand(Guid.NewGuid(), "Boeing", "SAS", 20);
        var createAirplaneHandler = new CreateAirplaneCommandHandler(client);
        var airplaneResponse = await createAirplaneHandler.Handle(createAirplaneRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createFlightRequest = new CreateFlightCommand(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2),
            "Kastrup", "Karup", Guid.Parse(airplaneResponse.Body!.Id), 1200);
        var createFlightHandler = new CreateFlightCommandHandler(client, Mock.Of<ILogger<CreateFlightCommandHandler>>());

        //Act
        await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);
        WaitForIndexing(store);
        var flightResponse = await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.Conflict, flightResponse.ResponseCode);
    }
}