using CarService.Domain;
using CarService.Infrastructure.Requests.CreateRentalCar;
using CarService.Infrastructure.Requests.CreateRentalDeal;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace CarService.Tests;

public class CreateRentalDealTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_CreateDeal()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();

        var createRentalCarHandler = new CreateRentalCarRequestHandler(session);
        var rentalCar = await createRentalCarHandler.Handle(
            new CreateRentalCarRequest(Guid.NewGuid(), "Mercedes", "EuropeCar", 12, "Blue"), CancellationToken.None);

        WaitForIndexing(store, timeout: TimeSpan.FromSeconds(5));
        
        var createDentalDealHandler = new CreateRentalDealRequestHandler(session);

        //Act
        var expected = await createDentalDealHandler.Handle(
            new CreateRentalDealRequest(DateTimeOffset.UtcNow.AddDays(1), DateTimeOffset.UtcNow.AddDays(2),
                rentalCar.Id), CancellationToken.None);
        
        WaitForIndexing(store, timeout: TimeSpan.FromSeconds(5));

        var actual = await session.Query<RentalDeal>().Where(x => x.Id == expected!.Id).FirstAsync();

        //Assert
        Assert.Equal(expected!.Id, actual.Id);
    }

    [Trait("Category", "Unit")]
    [Theory]
    [InlineData(5, 8, 3, 6)]
    [InlineData(20, 22, 21, 25)]
    public async Task Handle_CreateConflictingDeal(int from, int to, int conflictingFrom, int conflictingTo)
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();

        var rentalCarHandler = new CreateRentalCarRequestHandler(session);
        var rentalCar = await rentalCarHandler.Handle(
            new CreateRentalCarRequest(Guid.NewGuid(), "Mercedes", "EuropeCar", 12, "Blue"), CancellationToken.None);
        
        WaitForIndexing(store, timeout: TimeSpan.FromSeconds(5));
        
        var createRentalDealHandler = new CreateRentalDealRequestHandler(session);

        //Act
        await createRentalDealHandler.Handle(
            new CreateRentalDealRequest(DateTimeOffset.UtcNow.AddDays(from), DateTimeOffset.UtcNow.AddDays(to),
                rentalCar.Id), CancellationToken.None);

        WaitForIndexing(store, timeout: TimeSpan.FromSeconds(5));
        
        var conflictingRentalDeal = await createRentalDealHandler.Handle(
            new CreateRentalDealRequest(DateTimeOffset.UtcNow.AddDays(conflictingFrom), DateTimeOffset.UtcNow.AddDays(conflictingTo),
                rentalCar.Id), CancellationToken.None);

        //Assert
        Assert.Null(conflictingRentalDeal);
    }
}