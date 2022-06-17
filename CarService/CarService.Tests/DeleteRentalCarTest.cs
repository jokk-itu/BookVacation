using CarService.Domain;
using CarService.Infrastructure.Requests.CreateRentalCar;
using CarService.Infrastructure.Requests.CreateRentalDeal;
using CarService.Infrastructure.Requests.DeleteRentalDeal;
using Mediator;
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

        var deleteRentalDealHandler = new DeleteRentalDealRequestHandler(client, Mock.Of<ILogger<DeleteRentalDealRequestHandler>>());
        var createRentalDealHandler = new CreateRentalDealRequestHandler(client, Mock.Of<ILogger<CreateRentalDealRequestHandler>>());
        var rentalCarHandler = new CreateRentalCarRequestHandler(client);

        var rentalCarResponse =
            await rentalCarHandler.Handle(
                new CreateRentalCarRequest(Guid.NewGuid(), "Mercedes", "EuropeCar", 12, "Blue"),
                CancellationToken.None);

        WaitForIndexing(store);

        var rentalDealResponse = await createRentalDealHandler.Handle(
            new CreateRentalDealRequest(new DateTimeOffset().AddDays(1), new DateTimeOffset().AddDays(2),
                Guid.Parse(rentalCarResponse.Body!.Id)),
            CancellationToken.None);

        WaitForIndexing(store);

        //Act
        await deleteRentalDealHandler.Handle(new DeleteRentalDealRequest(Guid.Parse(rentalDealResponse.Body!.Id)),
            CancellationToken.None);

        WaitForIndexing(store);

        var deletedRentalDeal =
            await session.Query<RentalDeal>().Where(x => x.Id == rentalDealResponse.Body!.Id).FirstOrDefaultAsync();

        //Assert
        Assert.Null(deletedRentalDeal);
    }

    [Trait("Category", "Unit")]
    public async Task Handle_GiveNonExistingRentalDealId_ExpectNotFound()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());

        var deleteRentalDealHandler = new DeleteRentalDealRequestHandler(client, Mock.Of<ILogger<DeleteRentalDealRequestHandler>>());

        WaitForIndexing(store);

        //Act
        var rentalDealId = Guid.NewGuid();
        var actual = await deleteRentalDealHandler.Handle(new DeleteRentalDealRequest(rentalDealId),
            CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.NotFound, actual.ResponseCode);
    }
}