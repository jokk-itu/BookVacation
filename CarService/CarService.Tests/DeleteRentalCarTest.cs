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

        var deleteRentalDealHandler = new DeleteRentalDealCommandHandler(client, Mock.Of<ILogger<DeleteRentalDealCommandHandler>>());
        var createRentalDealHandler = new CreateRentalDealCommandHandler(client, Mock.Of<ILogger<CreateRentalDealCommandHandler>>());
        var rentalCarHandler = new CreateRentalCarCommandHandler(client);

        var rentalCarResponse =
            await rentalCarHandler.Handle(
                new CreateRentalCarCommand(Guid.NewGuid(), "Mercedes", "EuropeCar", 12, "Blue"),
                CancellationToken.None);

        WaitForIndexing(store);

        var rentalDealResponse = await createRentalDealHandler.Handle(
            new CreateRentalDealCommand(new DateTimeOffset().AddDays(1), new DateTimeOffset().AddDays(2),
                Guid.Parse(rentalCarResponse.Body!.Id)),
            CancellationToken.None);

        WaitForIndexing(store);

        //Act
        await deleteRentalDealHandler.Handle(new DeleteRentalDealCommand(Guid.Parse(rentalDealResponse.Body!.Id)),
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

        var deleteRentalDealHandler = new DeleteRentalDealCommandHandler(client, Mock.Of<ILogger<DeleteRentalDealCommandHandler>>());

        WaitForIndexing(store);

        //Act
        var rentalDealId = Guid.NewGuid();
        var actual = await deleteRentalDealHandler.Handle(new DeleteRentalDealCommand(rentalDealId),
            CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.NotFound, actual.ResponseCode);
    }
}