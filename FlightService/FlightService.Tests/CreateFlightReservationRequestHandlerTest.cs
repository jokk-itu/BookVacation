using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentClient;
using FlightService.Domain;
using FlightService.Infrastructure.Requests.CreateAirplane;
using FlightService.Infrastructure.Requests.CreateFlight;
using FlightService.Infrastructure.Requests.CreateFlightReservation;
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
    public async Task Handle_InvalidFlightId_ExpectNull()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createFlightReservationRequest =
            new CreateFlightReservationRequest(Guid.Empty, Guid.Empty);
        var createFlightReservationHandler = new CreateFlightReservationRequestHandler(client);

        //Act
        var invalidFlight =
            await createFlightReservationHandler.Handle(createFlightReservationRequest, CancellationToken.None);

        //Assert
        Assert.Null(invalidFlight);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectFlightReservation()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createAirplaneRequest = new CreateAirplaneRequest(Guid.NewGuid(), "Boeing", "SAS", 3);
        var createAirplaneHandler = new CreateAirplaneRequestHandler(client);
        var airplane = await createAirplaneHandler.Handle(createAirplaneRequest, CancellationToken.None);

        var createFlightRequest = new CreateFlightRequest(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2),
            "Karup", "Kastrup", Guid.Parse(airplane.Id), 1200);
        var createFlightHandler = new CreateFlightRequestHandler(client);
        var flight = await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);

        var createFlightReservationRequest = new CreateFlightReservationRequest(Guid.Parse(airplane.Seats.First().Id), Guid.Parse(flight!.Id));
        var createFlightReservationHandler = new CreateFlightReservationRequestHandler(client);

        //Act
        var expected =
            await createFlightReservationHandler.Handle(createFlightReservationRequest, CancellationToken.None);
        var actual = await session.Query<FlightReservation>().Where(x => x.Id == expected!.Id).FirstOrDefaultAsync();

        //Assert
        Assert.NotNull(actual);
    }
}