using System;
using System.Threading;
using System.Threading.Tasks;
using CarService.Infrastructure.Requests;
using CarService.Infrastructure.Requests.DeleteRentCar;
using Moq;
using Neo4j.Driver;
using Xunit;

namespace CarService.Tests;

public class DeleteRentCarRequestHandlerTest
{
    private readonly Mock<IDriver> _fakeDriver;

    private readonly Mock<IAsyncSession> _fakeSession;

    private readonly Mock<IAsyncTransaction> _fakeTransaction;

    private readonly Mock<IResultCursor> _fakeResultCursor;

    public DeleteRentCarRequestHandlerTest()
    {
        _fakeDriver = new Mock<IDriver>();
        _fakeSession = new Mock<IAsyncSession>();
        _fakeTransaction = new Mock<IAsyncTransaction>();
        _fakeResultCursor = new Mock<IResultCursor>();

        //Setup mocks
        _fakeDriver.Setup(d => d.AsyncSession())
            .Returns(_fakeSession.Object)
            .Verifiable();
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectOk()
    {
        //Arrange
        var rentId = Guid.NewGuid();
        var command = new DeleteRentCarRequest(rentId);
        var expected = RequestResult.Ok;

        _fakeTransaction.Setup(t => t.CommitAsync())
            .Verifiable();
        _fakeResultCursor.Setup(rc => rc.FetchAsync()).ReturnsAsync(true)
            .Verifiable();
        _fakeTransaction.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(_fakeResultCursor.Object)
            .Verifiable();
        _fakeSession.Setup(s => s.WriteTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<bool>>>()))
            .Returns((Func<IAsyncTransaction, Task<bool>> func) => func(_fakeTransaction.Object))
            .Verifiable();
        _fakeDriver.Setup(d => d.AsyncSession())
            .Returns(_fakeSession.Object)
            .Verifiable();
        
        //Act
        var handler = new DeleteRentCarRequestHandler(_fakeDriver.Object);
        var actual = await handler.Handle(command, CancellationToken.None);

        //Assert
        _fakeDriver.Verify();
        _fakeSession.Verify();
        _fakeTransaction.Verify();
        _fakeResultCursor.Verify();
        Assert.Equal(expected, actual);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectError()
    {
        //Arrange
        var rentId = Guid.NewGuid();
        var command = new DeleteRentCarRequest(rentId);
        var expected = RequestResult.Error;

        _fakeTransaction.Setup(t => t.RollbackAsync())
            .Verifiable();
        _fakeResultCursor.Setup(rc => rc.FetchAsync()).ReturnsAsync(false)
            .Verifiable();
        _fakeTransaction.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(_fakeResultCursor.Object)
            .Verifiable();
        _fakeSession.Setup(s => s.WriteTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<bool>>>()))
            .Returns((Func<IAsyncTransaction, Task<bool>> func) => func(_fakeTransaction.Object))
            .Verifiable();
        _fakeDriver.Setup(d => d.AsyncSession())
            .Returns(_fakeSession.Object)
            .Verifiable();
        
        //Act
        var handler = new DeleteRentCarRequestHandler(_fakeDriver.Object);
        var actual = await handler.Handle(command, CancellationToken.None);

        //Assert
        _fakeDriver.Verify();
        _fakeSession.Verify();
        _fakeTransaction.Verify();
        _fakeResultCursor.Verify();
        Assert.Equal(expected, actual);
    }
}