using System;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Domain;
using FlightService.Infrastructure.Requests.CreateAirplane;
using FlightService.Infrastructure.Requests.CreateFlight;
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
        var createAirplaneRequest = new CreateAirplaneRequest(Guid.NewGuid(), "Boeing", "SAS", 20);
        var createAirplaneHandler = new CreateAirplaneRequestHandler(session);
        var airplane = await createAirplaneHandler.Handle(createAirplaneRequest, CancellationToken.None);
        WaitForIndexing(store);
        
        var createFlightRequest = new CreateFlightRequest(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2), "Kastrup", "Karup", airplane.Id, 1200);
        var createFlightHandler = new CreateFlightRequestHandler(session);

        //Act
        var expected = await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);
        WaitForIndexing(store);
        var actual = session.Query<Flight>().Where(x => x.Id == expected!.Id);

        //Assert
        Assert.NotNull(actual);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_InvalidAirplaneId_ExpectNull()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var createFlightRequest = new CreateFlightRequest(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2), "Kastrup", "Karup", Guid.Empty.ToString(), 1200);
        var createFlightHandler = new CreateFlightRequestHandler(session);
        
        //Act
        var invalidFlight = await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);
        
        //Assert
        Assert.Null(invalidFlight);
    }
    
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_AddIdenticalFlight_ExpectConflict()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var createAirplaneRequest = new CreateAirplaneRequest(Guid.NewGuid(), "Boeing", "SAS", 20);
        var createAirplaneHandler = new CreateAirplaneRequestHandler(session);
        var airplane = await createAirplaneHandler.Handle(createAirplaneRequest, CancellationToken.None);
        WaitForIndexing(store);
        
        var createFlightRequest = new CreateFlightRequest(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2), "Kastrup", "Karup", airplane.Id, 1200);
        var createFlightHandler = new CreateFlightRequestHandler(session);

        //Act
        await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);
        WaitForIndexing(store);
        var conflict = await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);

        //Assert
        Assert.Null(conflict);
    }
}