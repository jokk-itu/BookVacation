using HotelService.Domain;
using HotelService.Infrastructure.Requests.CreateHotel;
using HotelService.Infrastructure.Requests.CreateHotelRoomReservation;
using HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;
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

        var createHotelRequest = new CreateHotelRequest(3, "Denmark", "Copenhagen", "Rue");
        var createHotelRequestHandler = new CreateHotelRequestHandler(session);
        var hotel = await createHotelRequestHandler.Handle(createHotelRequest, CancellationToken.None);
        WaitForIndexing(store);

        var createHotelRoomReservationRequest = new CreateHotelRoomReservationRequest(hotel!.Id,
            hotel!.HotelRooms.First().Id, DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2));
        var createHotelRoomReservationRequestHandler = new CreateHotelRoomReservationRequestHandler(session);
        var hotelRoomReservation =
            await createHotelRoomReservationRequestHandler.Handle(createHotelRoomReservationRequest,
                CancellationToken.None);
        WaitForIndexing(store);

        var deleteHotelRoomReservationRequest = new DeleteHotelRoomReservationRequest(hotelRoomReservation!.Id);
        var deleteHotelRoomReservationRequestHandler = new DeleteHotelRoomReservationRequestHandler(session);

        //Act
        await deleteHotelRoomReservationRequestHandler.Handle(deleteHotelRoomReservationRequest,
            CancellationToken.None);
        WaitForIndexing(store);

        var isDeleted = session.Query<HotelRoomReservation>().Any(x => x.Id == hotelRoomReservation.Id);

        //Assert
        Assert.True(isDeleted);
    }
}