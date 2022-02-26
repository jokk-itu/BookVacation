using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Domain.Entities.Nodes;
using FlightService.Infrastructure.Requests;
using FlightService.Infrastructure.Requests.ReadFlights;
using Moq;
using Neo4j.Driver;
using Xunit;

namespace FlightService.Tests;

public class ReadFlightsRequestHandlerTest
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

        var command = new ReadFlightsRequest(0u, 2u);
        var flights = new List<Flight>
        {
            new()
            {
                Id = Guid.NewGuid(),
                From = DateTime.Now,
                To = DateTime.Now.AddDays(2)
            },
            new()
            {
                Id = Guid.NewGuid(),
                From = DateTime.Now,
                To = DateTime.Now.AddDays(1)
            }
        };

        var (expectedResult, expectedFlights) = (RequestResult.Ok, flights);

        fakeResult.Setup(r => r["id"])
            .Returns(It.IsAny<Guid>())
            .Verifiable();
        fakeResult.Setup(r => r["from"])
            .Returns(It.IsAny<DateTime>())
            .Verifiable();
        fakeResult.Setup(r => r["to"])
            .Returns(It.IsAny<DateTime>())
            .Verifiable();

        fakeResultCursor.Setup(rc => rc.Current)
            .Returns(fakeResult.Object)
            .Verifiable();
        fakeResultCursor.SetupSequence(rc => rc.FetchAsync())
            .ReturnsAsync(true)
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        fakeTransaction.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(fakeResultCursor.Object)
            .Verifiable();
        fakeSession.Setup(s => s.ReadTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<List<Flight>?>>>()))
            .Returns((Func<IAsyncTransaction, Task<List<Flight>?>> func) => func(fakeTransaction.Object))
            .Verifiable();
        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();

        //Act
        var handler = new ReadFlightsRequestHandler(fakeDriver.Object);
        var (actualResult, actualFlights) = await handler.Handle(command, CancellationToken.None);

        //Assert
        fakeDriver.Verify();
        fakeSession.Verify();
        fakeTransaction.Verify();
        fakeResultCursor.Verify();
        Assert.Equal(expectedFlights.Count, actualFlights!.Count());
        Assert.Equal(expectedResult, actualResult);
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

        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();

        var command = new ReadFlightsRequest(0u, 2u);

        (var expectedResult, IEnumerable<Flight>? expectedFlights) = (RequestResult.Error, null);

        fakeResultCursor.Setup(rc => rc.FetchAsync())
            .ReturnsAsync(false)
            .Verifiable();
        fakeTransaction.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(fakeResultCursor.Object)
            .Verifiable();
        fakeSession.Setup(s => s.ReadTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<List<Flight>?>>>()))
            .Returns((Func<IAsyncTransaction, Task<List<Flight>?>> func) => func(fakeTransaction.Object))
            .Verifiable();
        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();

        //Act
        var handler = new ReadFlightsRequestHandler(fakeDriver.Object);
        var (actualResult, actualFlights) = await handler.Handle(command, CancellationToken.None);

        //Assert
        fakeDriver.Verify();
        fakeSession.Verify();
        fakeTransaction.Verify();
        fakeResultCursor.Verify();
        Assert.Equal(expectedFlights, actualFlights);
        Assert.Equal(expectedResult, actualResult);
    }
}