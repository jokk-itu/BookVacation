using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Domain;
using FlightService.Infrastructure.Requests.CreateAirplane;
using FlightService.Infrastructure.Requests.CreateFlight;
using FlightService.Infrastructure.Requests.CreateFlightReservation;
using Mediator;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace FlightService.Tests;

public class CreateFlightReservationRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_InvalidFlightId_ExpectNotFound()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createFlightReservationRequest =
            new CreateFlightReservationCommand(Guid.NewGuid(), Guid.NewGuid());
        var createFlightReservationHandler =
            new CreateFlightReservationCommandHandler(client,
                Mock.Of<ILogger<CreateFlightReservationCommandHandler>>());

        //Act
        var flightResponse =
            await createFlightReservationHandler.Handle(createFlightReservationRequest, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.NotFound, flightResponse.ResponseCode);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectFlightReservation()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createAirplaneRequest = new CreateAirplaneCommand(Guid.NewGuid(), "Boeing", "SAS", 3);
        var createAirplaneHandler = new CreateAirplaneCommandHandler(client);
        var airplaneResponse = await createAirplaneHandler.Handle(createAirplaneRequest, CancellationToken.None);

        var createFlightRequest = new CreateFlightCommand(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2),
            "Karup", "Kastrup", Guid.Parse(airplaneResponse.Body!.Id), 1200);
        var createFlightHandler =
            new CreateFlightCommandHandler(client, Mock.Of<ILogger<CreateFlightCommandHandler>>());
        var flightResponse = await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);

        var createFlightReservationRequest =
            new CreateFlightReservationCommand(Guid.Parse(airplaneResponse.Body!.Seats.First().Id),
                Guid.Parse(flightResponse.Body!.Id));
        var createFlightReservationHandler =
            new CreateFlightReservationCommandHandler(client,
                Mock.Of<ILogger<CreateFlightReservationCommandHandler>>());

        //Act
        var flightReservationResponse =
            await createFlightReservationHandler.Handle(createFlightReservationRequest, CancellationToken.None);
        WaitForIndexing(store);
        var actual = await session.Query<FlightReservation>().Where(x => x.Id == flightReservationResponse.Body!.Id)
            .FirstOrDefaultAsync();

        //Assert
        Assert.NotNull(actual);
        Assert.Equal(ResponseCode.Ok, flightReservationResponse.ResponseCode);
        Assert.NotNull(flightReservationResponse.Body);
    }
}