using System;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Domain.Entities.Nodes;
using FlightService.Infrastructure.Requests;
using FlightService.Infrastructure.Requests.ReadFlight;
using Moq;
using Neo4j.Driver;
using Xunit;

namespace FlightService.Tests;

public class ReadFlightRequestHandlerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectOk()
    {
        //Arrange
        var fakeDriver = new Mock<IDriver>();
        var fakeSession = new Mock<IAsyncSession>();
        var fakeTransaction = new Mock<IAsyncTransaction>();
        var fakeResultCursor = new Mock<IResultCursor>();
        var fakeResult = new Mock<IRecord>();

        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();

        var command = new ReadFlightRequest(Guid.NewGuid());
        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            From = DateTime.Now,
            To = DateTime.Now
        };
        var (expectedResult, expectedFlight) = (RequestResult.Ok, flight);

        fakeResult.Setup(r => r["id"])
            .Returns(flight.Id)
            .Verifiable();
        fakeResult.Setup(r => r["from"])
            .Returns(flight.From)
            .Verifiable();
        fakeResult.Setup(r => r["to"])
            .Returns(flight.To)
            .Verifiable();

        fakeResultCursor.Setup(rc => rc.Current)
            .Returns(fakeResult.Object)
            .Verifiable();
        fakeResultCursor.Setup(rc => rc.FetchAsync())
            .ReturnsAsync(true)
            .Verifiable();
        fakeTransaction.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(fakeResultCursor.Object)
            .Verifiable();
        fakeSession.Setup(s => s.ReadTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<Flight?>>>()))
            .Returns((Func<IAsyncTransaction, Task<Flight?>> func) => func(fakeTransaction.Object))
            .Verifiable();
        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();

        //Act
        var handler = new ReadFlightRequestHandler(fakeDriver.Object);
        var (actualResult, actualFlight) = await handler.Handle(command, CancellationToken.None);

        //Assert
        fakeDriver.Verify();
        fakeSession.Verify();
        fakeTransaction.Verify();
        fakeResultCursor.Verify();
        Assert.NotNull(actualFlight);
        Assert.Equal(expectedResult, actualResult);
        Assert.Equal(expectedFlight.Id, actualFlight!.Id);
        Assert.Equal(expectedFlight.To, actualFlight.To);
        Assert.Equal(expectedFlight.From, actualFlight.From);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectNotFound()
    {
        //Arrange
        var fakeDriver = new Mock<IDriver>();
        var fakeSession = new Mock<IAsyncSession>();
        var fakeTransaction = new Mock<IAsyncTransaction>();
        var fakeResultCursor = new Mock<IResultCursor>();

        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();

        var command = new ReadFlightRequest(Guid.NewGuid());

        (var expectedResult, Flight? expectedFlight) = (RequestResult.NotFound, null);

        fakeResultCursor.Setup(rc => rc.FetchAsync())
            .ReturnsAsync(false)
            .Verifiable();
        fakeTransaction.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(fakeResultCursor.Object)
            .Verifiable();
        fakeSession.Setup(s => s.ReadTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<Flight?>>>()))
            .Returns((Func<IAsyncTransaction, Task<Flight?>> func) => func(fakeTransaction.Object))
            .Verifiable();
        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();

        //Act
        var handler = new ReadFlightRequestHandler(fakeDriver.Object);
        var (actualResult, actualFlight) = await handler.Handle(command, CancellationToken.None);

        //Assert
        fakeDriver.Verify();
        fakeSession.Verify();
        fakeTransaction.Verify();
        fakeResultCursor.Verify();
        Assert.Equal(expectedFlight, actualFlight);
        Assert.Equal(expectedResult, actualResult);
    }
}