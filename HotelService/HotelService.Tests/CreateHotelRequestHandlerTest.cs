using HotelService.Infrastructure.Requests.CreateHotel;
using Mediator;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.TestDriver;
using Xunit;

namespace HotelService.Tests;

public class CreateHotelRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectHotel()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var request = new CreateHotelCommand(30, "Denmark", "Copenhagen", "Rue");
        var handler = new CreateHotelCommandHandler(client, Mock.Of<ILogger<CreateHotelCommandHandler>>());

        //Act
        var hotelResponse = await handler.Handle(request, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.Ok, hotelResponse.ResponseCode);
        Assert.NotNull(hotelResponse.Body);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenConflicting_ExpectNull()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var request = new CreateHotelCommand(30, "Denmark", "Copenhagen", "Rue");
        var handler = new CreateHotelCommandHandler(client, Mock.Of<ILogger<CreateHotelCommandHandler>>());
        await handler.Handle(request, CancellationToken.None);
        WaitForIndexing(store);

        //Act
        var hotelResponse = await handler.Handle(request, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.Conflict, hotelResponse.ResponseCode);
        Assert.Null(hotelResponse.Body);
    }
}