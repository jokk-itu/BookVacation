using HotelService.Domain;
using HotelService.Infrastructure.Requests.CreateHotel;
using Raven.Client.Documents;
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
        var request = new CreateHotelRequest(30, "Denmark", "Copenhagen", "Rue");
        var handler = new CreateHotelRequestHandler(session);

        //Act
        var expect = await handler.Handle(request, CancellationToken.None);
        WaitForIndexing(store);
        var actual = await session.Query<Hotel>().Where(x => x.Id == expect!.Id).FirstOrDefaultAsync();

        //Assert
        Assert.NotNull(actual);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenConflicting_ExpectNull()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var request = new CreateHotelRequest(30, "Denmark", "Copenhagen", "Rue");
        var handler = new CreateHotelRequestHandler(session);
        await handler.Handle(request, CancellationToken.None);
        WaitForIndexing(store);

        //Act
        var conflictingHotel = await handler.Handle(request, CancellationToken.None);

        //Assert
        Assert.Null(conflictingHotel);
    }
}