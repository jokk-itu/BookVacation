using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Domain;
using FlightService.Infrastructure.Requests.CreateAirplane;
using FlightService.Infrastructure.Requests.CreateFlight;
using FlightService.Infrastructure.Requests.CreateFlightReservation;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace FlightService.Tests;

public class CreateFlightReservationRequestHandlerTest : RavenTestDriver
{
    //Execute, invalid flight
    
    //Execute, invalid seat
    
    //Execute, conflictingFightReservation
    
    //Execute, Correct
    
    //Compensate, Correct
    
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_InvalidFlightId_ExpectNull()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();

        var createFlightReservationRequest = new CreateFlightReservationRequest(Guid.Empty.ToString(), Guid.Empty.ToString());
        var createFlightReservationHandler = new CreateFlightReservationRequestHandler(session);
        
        //Act
        var invalidFlight = await createFlightReservationHandler.Handle(createFlightReservationRequest, CancellationToken.None);

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

        var createAirplaneRequest = new CreateAirplaneRequest(Guid.NewGuid(), "Boeing", "SAS", 20);
        var createAirplaneHandler = new CreateAirplaneRequestHandler(session);
        var airplane = await createAirplaneHandler.Handle(createAirplaneRequest, CancellationToken.None);
        
        var createFlightRequest = new CreateFlightRequest(DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2), "Karup", "Kastrup", airplane.Id, 1200);
        var createFlightHandler = new CreateFlightRequestHandler(session);
        var flight = await createFlightHandler.Handle(createFlightRequest, CancellationToken.None);
        
        var createFlightReservationRequest = new CreateFlightReservationRequest(airplane.Seats.First().Id, flight!.Id);
        var createFlightReservationHandler = new CreateFlightReservationRequestHandler(session);
        
        //Act
        var expected =
            await createFlightReservationHandler.Handle(createFlightReservationRequest, CancellationToken.None);
        var actual = await session.Query<FlightReservation>().Where(x => x.Id == expected!.Id).FirstOrDefaultAsync();
        
        //Assert
        Assert.NotNull(actual);
    }
}