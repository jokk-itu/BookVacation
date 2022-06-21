using HotelService.Domain;
using HotelService.Infrastructure.Requests.CreateHotel;
using HotelService.Infrastructure.Requests.CreateHotelRoomReservation;
using Mediator;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes.TimeSeries;
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
        var createHotelRequest = new CreateHotelCommand(3, "Denmark", "Copenhagen", "Rue");
        var createHotelRequestHandler = new CreateHotelCommandHandler(client, Mock.Of<ILogger<CreateHotelCommandHandler>>());
        var hotelResponse = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationCommand(Guid.Parse(hotelResponse.Body!.Id),
            Guid.Parse(hotelResponse.Body!.HotelRooms.First().Id), DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler = new CreateHotelRoomReservationCommandHandler(client, Mock.Of<ILogger<CreateHotelRoomReservationCommandHandler>>());

        //Act
        var hotelRoomReservationResponse =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.Ok, hotelRoomReservationResponse.ResponseCode);
        Assert.NotNull(hotelRoomReservationResponse.Body);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenInvalidHotel_ExpectNotFound()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createHotelRequest = new CreateHotelCommand(3, "Denmark", "Copenhagen", "Rue");
        var createHotelRequestHandler = new CreateHotelCommandHandler(client, Mock.Of<ILogger<CreateHotelCommandHandler>>());
        var hotelResponse = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationCommand(Guid.Empty,
            Guid.Parse(hotelResponse.Body!.HotelRooms.First().Id), DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler = new CreateHotelRoomReservationCommandHandler(client, Mock.Of<ILogger<CreateHotelRoomReservationCommandHandler>>());

        //Act
        var hotelRoomReservationResponse =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.NotFound, hotelRoomReservationResponse.ResponseCode);
        Assert.Null(hotelRoomReservationResponse.Body);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenInvalidRoomId_ExpectNotFound()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createHotelRequest = new CreateHotelCommand(3, "Denmark", "Copenhagen", "Rue");
        var createHotelRequestHandler = new CreateHotelCommandHandler(client, Mock.Of<ILogger<CreateHotelCommandHandler>>());
        var hotelResponse = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationCommand(Guid.Parse(hotelResponse.Body!.Id), Guid.Empty,
            DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler = new CreateHotelRoomReservationCommandHandler(client, Mock.Of<ILogger<CreateHotelRoomReservationCommandHandler>>());

        //Act
        var hotelRoomReservationResponse =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.NotFound, hotelRoomReservationResponse.ResponseCode);
        Assert.Null(hotelRoomReservationResponse.Body);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenConflictingHotelRoomReservation_ExpectConflict()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createHotelRequest = new CreateHotelCommand(3, "Denmark", "Copenhagen", "Rue");
        var createHotelRequestHandler = new CreateHotelCommandHandler(client, Mock.Of<ILogger<CreateHotelCommandHandler>>());
        var hotelResponse = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationCommand(Guid.Parse(hotelResponse.Body!.Id),
            Guid.Parse(hotelResponse.Body!.HotelRooms.First().Id), DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler = new CreateHotelRoomReservationCommandHandler(client, Mock.Of<ILogger<CreateHotelRoomReservationCommandHandler>>());
        await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
            CancellationToken.None);
        WaitForIndexing(store);

        //Act
        var hotelRoomReservationResponse =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.Conflict, hotelRoomReservationResponse.ResponseCode);
        Assert.Null(hotelRoomReservationResponse.Body);
    }
}