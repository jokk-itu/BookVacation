using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Infrastructure.Requests.CreateAirplane;
using FlightService.Infrastructure.Requests.CreateFlight;
using FlightService.Infrastructure.Requests.CreateFlightReservation;
using FlightService.Infrastructure.Requests.DeleteFlightReservation;
using Mediator;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.TestDriver;
using Xunit;

namespace FlightService.Tests;

public class DeleteFlightReservationRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectOk()
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
        var createFlightHandler = new CreateFlightCommandHandler(client, Mock.Of<ILogger<CreateFlightCommandHandler>>());
        var flightResponse = await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);

        var createFlightReservationRequest =
            new CreateFlightReservationCommand(Guid.Parse(airplaneResponse.Body!.Seats.First().Id), Guid.Parse(flightResponse.Body!.Id));
        var createFlightReservationHandler = new CreateFlightReservationCommandHandler(client, Mock.Of<ILogger<CreateFlightReservationCommandHandler>>());
        var flightReservationResponse =
            await createFlightReservationHandler.Handle(createFlightReservationRequest, CancellationToken.None);

        //Act
        var deleteFlightReservationRequest = new DeleteFlightReservationCommand(Guid.Parse(flightReservationResponse.Body!.Id));
        var deleteFlightReservationHandler =
            new DeleteFlightReservationCommandHandler(client,
                Mock.Of<ILogger<DeleteFlightReservationCommandHandler>>());
        var response = await deleteFlightReservationHandler.Handle(deleteFlightReservationRequest, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.Ok, response.ResponseCode);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectNotFound()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
       
        //Act
        var deleteFlightReservationRequest = new DeleteFlightReservationCommand(Guid.NewGuid());
        var deleteFlightReservationHandler =
            new DeleteFlightReservationCommandHandler(client,
                Mock.Of<ILogger<DeleteFlightReservationCommandHandler>>());
        var response = await deleteFlightReservationHandler.Handle(deleteFlightReservationRequest, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.NotFound, response.ResponseCode);
    }
}