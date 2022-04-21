using CarService.Domain;
using CarService.Infrastructure.Requests.CreateRentalCar;
using CarService.Infrastructure.Requests.CreateRentalDeal;
using CarService.Infrastructure.Requests.DeleteRentalDeal;
using DocumentClient;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace CarService.Tests;

public class DeleteRentalCarTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_CreateRentalDeal_ExpectDeleteSuccessfully()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        
        var deleteRentalDealHandler = new DeleteRentalDealRequestHandler(client);
        var createRentalDealHandler = new CreateRentalDealRequestHandler(client);
        var rentalCarHandler = new CreateRentalCarRequestHandler(client);

        var rentalCar =
            await rentalCarHandler.Handle(
                new CreateRentalCarRequest(Guid.NewGuid(), "Mercedes", "EuropeCar", 12, "Blue"),
                CancellationToken.None);

        WaitForIndexing(store);

        var rentalDeal = await createRentalDealHandler.Handle(
            new CreateRentalDealRequest(new DateTimeOffset().AddDays(1), new DateTimeOffset().AddDays(2), Guid.Parse(rentalCar.Id)),
            CancellationToken.None);

        WaitForIndexing(store);

        //Act
        await deleteRentalDealHandler.Handle(new DeleteRentalDealRequest(Guid.Parse(rentalDeal!.Id)), CancellationToken.None);

        WaitForIndexing(store);

        var deletedRentalDeal =
            await session.Query<RentalDeal>().Where(x => x.Id == rentalDeal.Id).FirstOrDefaultAsync();

        //Assert
        Assert.Null(deletedRentalDeal);
    }
}