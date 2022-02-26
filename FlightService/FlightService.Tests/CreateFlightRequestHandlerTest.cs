using System;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Domain.Entities.Nodes;
using FlightService.Infrastructure.Requests;
using FlightService.Infrastructure.Requests.CreateFlight;
using FlightService.Infrastructure.Requests.ReadFlight;
using MediatR;
using Moq;
using Neo4j.Driver;
using Xunit;

namespace FlightService.Tests;

public class CreateFlightRequestHandlerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectCreated()
    {
        //Arrange
        var fakeDriver = new Mock<IDriver>();
        var fakeSession = new Mock<IAsyncSession>();
        var fakeTransaction = new Mock<IAsyncTransaction>();
        var fakeResultCursor = new Mock<IResultCursor>();
        var fakeResult = new Mock<IRecord>();
        var fakeMediator = new Mock<IMediator>();

        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();
        fakeMediator.Setup(m => m.Send(It.IsAny<ReadFlightRequest>(), CancellationToken.None))
            .ReturnsAsync((RequestResult.Ok, It.IsAny<Flight>()))
            .Verifiable();

        var command = new CreateFlightRequest(DateTime.Today, DateTime.Today.AddDays(2));
        var flight = new Flight
        {
            Id = Guid.NewGuid(),
            From = DateTime.Now,
            To = DateTime.Now
        };
        var (expectedResult, expectedFlight) = (RequestResult.Created, flight);

        fakeResult.Setup(r => r.Values["id"])
            .Returns(flight.Id)
            .Verifiable();
        fakeResult.Setup(r => r.Values["from"])
            .Returns(flight.From)
            .Verifiable();
        fakeResult.Setup(r => r.Values["to"])
            .Returns(flight.To)
            .Verifiable();

        fakeResultCursor.Setup(rc => rc.Current)
            .Returns(fakeResult.Object)
            .Verifiable();
        fakeTransaction.Setup(t => t.CommitAsync())
            .Verifiable();
        fakeResultCursor.Setup(rc => rc.FetchAsync())
            .ReturnsAsync(true)
            .Verifiable();
        fakeTransaction.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(fakeResultCursor.Object)
            .Verifiable();
        fakeSession.Setup(s => s.WriteTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<Flight?>>>()))
            .Returns((Func<IAsyncTransaction, Task<Flight?>> func) => func(fakeTransaction.Object))
            .Verifiable();
        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();

        //Act
        var handler = new CreateFlightRequestHandler(fakeDriver.Object);
        var (actualResult, actualFlight) = await handler.Handle(command, CancellationToken.None);

        //Assert
        fakeDriver.Verify();
        fakeSession.Verify();
        fakeTransaction.Verify();
        fakeResultCursor.Verify();
        Assert.NotNull(actualFlight);
        Assert.Equal(expectedResult, actualResult);
        Assert.Equal(expectedFlight.Id, actualFlight!.Id);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectError()
    {
        //Arrange
        var fakeDriver = new Mock<IDriver>();
        var fakeSession = new Mock<IAsyncSession>();
        var fakeTransaction = new Mock<IAsyncTransaction>();
        var fakeResultCursor = new Mock<IResultCursor>();
        var fakeMediator = new Mock<IMediator>();

        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();
        fakeMediator.Setup(m => m.Send(It.IsAny<ReadFlightRequest>(), CancellationToken.None))
            .ReturnsAsync((RequestResult.Ok, It.IsAny<Flight>()))
            .Verifiable();

        var command = new CreateFlightRequest(DateTime.Today, DateTime.Today.AddDays(2));
        (RequestResult, Flight?) expected = (RequestResult.Error, null);

        fakeTransaction.Setup(t => t.RollbackAsync())
            .Verifiable();
        fakeResultCursor.Setup(rc => rc.FetchAsync())
            .ReturnsAsync(false)
            .Verifiable();
        fakeTransaction.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(fakeResultCursor.Object)
            .Verifiable();
        fakeSession.Setup(s => s.WriteTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<Flight?>>>()))
            .Returns((Func<IAsyncTransaction, Task<Flight?>> func) => func(fakeTransaction.Object))
            .Verifiable();
        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();

        //Act
        var handler = new CreateFlightRequestHandler(fakeDriver.Object);
        var actual = await handler.Handle(command, CancellationToken.None);

        //Assert
        fakeDriver.Verify();
        fakeSession.Verify();
        fakeTransaction.Verify();
        fakeResultCursor.Verify();
        Assert.Equal(expected.Item1, actual.Item1);
        Assert.Null(actual.Item2);
    }
}