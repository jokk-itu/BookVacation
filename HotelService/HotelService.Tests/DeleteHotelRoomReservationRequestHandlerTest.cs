using HotelService.Domain;
using HotelService.Infrastructure.Requests.CreateHotel;
using HotelService.Infrastructure.Requests.CreateHotelRoomReservation;
using HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;
using Mediator;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace HotelService.Tests;

public class DeleteHotelRoomReservationRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectDeleted()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createHotelRequest = new CreateHotelCommand(3, "Denmark", "Copenhagen", "Rue");
        var createHotelRequestHandler =
            new CreateHotelCommandHandler(client, Mock.Of<ILogger<CreateHotelCommandHandler>>());
        var hotelResponse = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationCommand(
            Guid.Parse(hotelResponse.Body!.Id),
            Guid.Parse(hotelResponse.Body!.HotelRooms.First().Id), DateTimeOffset.Now.AddDays(1),
            DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler =
            new CreateHotelRoomReservationCommandHandler(client,
                Mock.Of<ILogger<CreateHotelRoomReservationCommandHandler>>());
        var hotelRoomReservation =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);
        WaitForIndexing(store);

        var deleteHotelRoomReservationCommand =
            new DeleteHotelRoomReservationCommand(Guid.Parse(hotelRoomReservation.Body!.Id));
        var deleteHotelRoomReservationRequestHandler =
            new DeleteHotelRoomReservationCommandHandler(client,
                Mock.Of<ILogger<DeleteHotelRoomReservationCommandHandler>>());

        //Act
        var response = await deleteHotelRoomReservationRequestHandler.Handle(deleteHotelRoomReservationCommand,
            CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.Ok, response.ResponseCode);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectNotFound()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var deleteHotelRoomReservationRequestHandler =
            new DeleteHotelRoomReservationCommandHandler(client,
                Mock.Of<ILogger<DeleteHotelRoomReservationCommandHandler>>());
        var command = new DeleteHotelRoomReservationCommand(Guid.NewGuid());

        //Act
        var response = await deleteHotelRoomReservationRequestHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.NotFound, response.ResponseCode);
    }
}