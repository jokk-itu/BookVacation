using CarService.Domain;
using CarService.Infrastructure.Requests.CreateRentalCar;
using CarService.Infrastructure.Requests.CreateRentalDeal;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace CarService.Tests;

public class CreateRentalDealTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_CreateDeal_ExpectRentalDeal()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();

        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createRentalCarHandler = new CreateRentalCarRequestHandler(client);
        var rentalCarResponse = await createRentalCarHandler.Handle(
            new CreateRentalCarRequest(Guid.NewGuid(), "Mercedes", "EuropeCar", 12, "Blue"), CancellationToken.None);

        WaitForIndexing(store);

        var createDentalDealHandler = new CreateRentalDealRequestHandler(client, Mock.Of<ILogger<CreateRentalDealRequestHandler>>());

        //Act
        var rentalDealResponse = await createDentalDealHandler.Handle(
            new CreateRentalDealRequest(DateTimeOffset.UtcNow.AddDays(1), DateTimeOffset.UtcNow.AddDays(2),
                Guid.Parse(rentalCarResponse.Body!.Id)), CancellationToken.None);

        WaitForIndexing(store);

        var actual = await session.Query<RentalDeal>().Where(x => x.Id == rentalDealResponse.Body!.Id).FirstAsync();

        //Assert
        Assert.Equal(rentalDealResponse.Body!.Id, actual.Id);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_CreateIncorrectDeal_ExpectNull()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();

        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var createDentalDealHandler = new CreateRentalDealRequestHandler(client, Mock.Of<ILogger<CreateRentalDealRequestHandler>>());

        //Act
        var invalidRentalDeal = await createDentalDealHandler.Handle(
            new CreateRentalDealRequest(DateTimeOffset.UtcNow.AddDays(1), DateTimeOffset.UtcNow.AddDays(2),
                Guid.Empty), CancellationToken.None);

        //Assert
        Assert.Null(invalidRentalDeal);
    }

    [Trait("Category", "Unit")]
    [Theory]
    [InlineData(5, 8, 3, 6)]
    [InlineData(20, 22, 21, 25)]
    public async Task Handle_CreateConflictingDeal_ExpectNull(int from, int to, int conflictingFrom, int conflictingTo)
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();

        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var rentalCarHandler = new CreateRentalCarRequestHandler(client);
        var rentalCarResponse = await rentalCarHandler.Handle(
            new CreateRentalCarRequest(Guid.NewGuid(), "Mercedes", "EuropeCar", 12, "Blue"), CancellationToken.None);

        WaitForIndexing(store);

        var createRentalDealHandler = new CreateRentalDealRequestHandler(client, Mock.Of<ILogger<CreateRentalDealRequestHandler>>());

        //Act
        await createRentalDealHandler.Handle(
            new CreateRentalDealRequest(DateTimeOffset.UtcNow.AddDays(from), DateTimeOffset.UtcNow.AddDays(to),
                Guid.Parse(rentalCarResponse.Body!.Id)), CancellationToken.None);

        WaitForIndexing(store);

        var conflictingRentalDeal = await createRentalDealHandler.Handle(
            new CreateRentalDealRequest(DateTimeOffset.UtcNow.AddDays(conflictingFrom),
                DateTimeOffset.UtcNow.AddDays(conflictingTo),
                Guid.Parse(rentalCarResponse.Body!.Id)), CancellationToken.None);

        //Assert
        Assert.Null(conflictingRentalDeal);
    }
}