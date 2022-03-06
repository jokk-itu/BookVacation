using CarService.Domain;
using CarService.Infrastructure.Requests.CreateRentalCar;
using CarService.Infrastructure.Requests.CreateRentalDeal;
using CarService.Infrastructure.Requests.DeleteRentalDeal;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace CarService.Tests;

public class DeleteRentalCarTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_CreateRentalDeal_DeleteSuccessfully()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();

        var deleteRentalDealHandler = new DeleteRentalDealRequestHandler(session);
        var createRentalDealHandler = new CreateRentalDealRequestHandler(session);
        var rentalCarHandler = new CreateRentalCarRequestHandler(session);

        var rentalCar =
            await rentalCarHandler.Handle(
                new CreateRentalCarRequest(Guid.NewGuid(), "Mercedes", "EuropeCar", 12, "Blue"),
                CancellationToken.None);

        WaitForIndexing(store, timeout: TimeSpan.FromSeconds(5));

        var rentalDeal = await createRentalDealHandler.Handle(
            new CreateRentalDealRequest(new DateTimeOffset().AddDays(1), new DateTimeOffset().AddDays(2), rentalCar.Id),
            CancellationToken.None);

        WaitForIndexing(store, timeout: TimeSpan.FromSeconds(5));

        //Act
        await deleteRentalDealHandler.Handle(new DeleteRentalDealRequest(rentalDeal!.Id), CancellationToken.None);

        WaitForIndexing(store, timeout: TimeSpan.FromSeconds(5));

        var deletedRentalDeal =
            await session.Query<RentalDeal>().Where(x => x.Id == rentalDeal.Id).FirstOrDefaultAsync();

        //Assert
        Assert.Null(deletedRentalDeal);
    }
}