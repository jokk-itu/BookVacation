using HotelService.Domain;
using HotelService.Infrastructure.Requests.CreateHotel;
using HotelService.Infrastructure.Requests.CreateHotelRoomReservation;
using HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;
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
        var createHotelRequestHandler = new CreateHotelCommandHandler(client);
        var hotel = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationCommand(Guid.Parse(hotel!.Id),
            Guid.Parse(hotel!.HotelRooms.First().Id), DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler = new CreateHotelRoomReservationCommandHandler(client);
        var hotelRoomReservation =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);
        WaitForIndexing(store);

        var deleteHotelRoomReservationRequest =
            new DeleteHotelRoomReservationCommand(Guid.Parse(hotelRoomReservation!.Id));
        var deleteHotelRoomReservationRequestHandler = new DeleteHotelRoomReservationCommandHandler(client);

        //Act
        await deleteHotelRoomReservationRequestHandler.Handle(deleteHotelRoomReservationRequest,
            CancellationToken.None);
        WaitForIndexing(store);

        var deletedReservationExists =
            await session.Query<HotelRoomReservation>().AnyAsync(x => x.Id == hotelRoomReservation.Id);

        //Assert
        Assert.False(deletedReservationExists);
    }
}