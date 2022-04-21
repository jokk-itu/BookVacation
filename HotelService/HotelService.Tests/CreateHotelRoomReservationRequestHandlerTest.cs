using HotelService.Domain;
using HotelService.Infrastructure.Requests.CreateHotel;
using HotelService.Infrastructure.Requests.CreateHotelRoomReservation;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace HotelService.Tests;

public class CreateHotelRoomReservationRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectHotelRoomReservation()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createHotelRequest = new CreateHotelRequest(3, "Denmark", "Copenhagen", "Rue");
        var createHotelRequestHandler = new CreateHotelRequestHandler(client);
        var hotel = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationRequest(Guid.Parse(hotel!.Id),
            Guid.Parse(hotel!.HotelRooms.First().Id), DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler = new CreateHotelRoomReservationRequestHandler(client);

        //Act
        var expected =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);
        WaitForIndexing(store);
        var actual = await session.Query<HotelRoomReservation>().Where(x => x.Id == expected!.Id).FirstOrDefaultAsync();

        //Assert
        Assert.NotNull(actual);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenInvalidHotel_ExpectNull()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createHotelRequest = new CreateHotelRequest(3, "Denmark", "Copenhagen", "Rue");
        var createHotelRequestHandler = new CreateHotelRequestHandler(client);
        var hotel = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationRequest(Guid.Empty,
            Guid.Parse(hotel!.HotelRooms.First().Id), DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler = new CreateHotelRoomReservationRequestHandler(client);

        //Act
        var conflictingHotelRoomReservation =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);

        //Assert
        Assert.Null(conflictingHotelRoomReservation);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenInvalidRoomId_ExpectNull()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createHotelRequest = new CreateHotelRequest(3, "Denmark", "Copenhagen", "Rue");
        var createHotelRequestHandler = new CreateHotelRequestHandler(client);
        var hotel = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationRequest(Guid.Parse(hotel!.Id), Guid.Empty,
            DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler = new CreateHotelRoomReservationRequestHandler(client);

        //Act
        var conflictingHotelRoomReservation =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);

        //Assert
        Assert.Null(conflictingHotelRoomReservation);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_DoConflictingHotelRoomReservation_ExpectNull()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createHotelRequest = new CreateHotelRequest(3, "Denmark", "Copenhagen", "Rue");
        var createHotelRequestHandler = new CreateHotelRequestHandler(client);
        var hotel = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationRequest(Guid.Parse(hotel!.Id),
            Guid.Parse(hotel.HotelRooms.First().Id), DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler = new CreateHotelRoomReservationRequestHandler(client);
        await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
            CancellationToken.None);
        WaitForIndexing(store);

        //Act
        var conflictingHotelRoomReservation =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);

        //Assert
        Assert.Null(conflictingHotelRoomReservation);
    }
}