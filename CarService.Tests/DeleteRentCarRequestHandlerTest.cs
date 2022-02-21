using CarService.Infrastructure.Requests;
using CarService.Infrastructure.Requests.DeleteRentCar;
using Moq;
using Neo4j.Driver;
using Xunit;

namespace CarService.Tests;

public class DeleteRentCarRequestHandlerTest
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
        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();
        var rentId = Guid.NewGuid();
        var command = new DeleteRentCarRequest(rentId);
        var expected = RequestResult.Ok;

        fakeTransaction.Setup(t => t.CommitAsync())
            .Verifiable();
        fakeResultCursor.Setup(rc => rc.FetchAsync()).ReturnsAsync(true)
            .Verifiable();
        fakeTransaction.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(fakeResultCursor.Object)
            .Verifiable();
        fakeSession.Setup(s => s.WriteTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<bool>>>()))
            .Returns((Func<IAsyncTransaction, Task<bool>> func) => func(fakeTransaction.Object))
            .Verifiable();
        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();
        
        //Act
        var handler = new DeleteRentCarRequestHandler(fakeDriver.Object);
        var actual = await handler.Handle(command, CancellationToken.None);

        //Assert
        fakeDriver.Verify();
        fakeSession.Verify();
        fakeTransaction.Verify();
        fakeResultCursor.Verify();
        Assert.Equal(expected, actual);
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
        var rentId = Guid.NewGuid();
        var command = new DeleteRentCarRequest(rentId);
        var expected = RequestResult.Error;

        fakeTransaction.Setup(t => t.RollbackAsync())
            .Verifiable();
        fakeResultCursor.Setup(rc => rc.FetchAsync()).ReturnsAsync(false)
            .Verifiable();
        fakeTransaction.Setup(t => t.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(fakeResultCursor.Object)
            .Verifiable();
        fakeSession.Setup(s => s.WriteTransactionAsync(It.IsAny<Func<IAsyncTransaction, Task<bool>>>()))
            .Returns((Func<IAsyncTransaction, Task<bool>> func) => func(fakeTransaction.Object))
            .Verifiable();
        fakeDriver.Setup(d => d.AsyncSession())
            .Returns(fakeSession.Object)
            .Verifiable();
        
        //Act
        var handler = new DeleteRentCarRequestHandler(fakeDriver.Object);
        var actual = await handler.Handle(command, CancellationToken.None);

        //Assert
        fakeDriver.Verify();
        fakeSession.Verify();
        fakeTransaction.Verify();
        fakeResultCursor.Verify();
        Assert.Equal(expected, actual);
    }
}