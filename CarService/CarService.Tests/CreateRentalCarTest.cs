using CarService.Domain;
using CarService.Infrastructure.Requests.CreateRentalCar;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace CarService.Tests;

public class CreateRentalCarTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectRentalCar()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();

        var request = new CreateRentalCarRequest(Guid.NewGuid(), "Mercedes", "EuropeCar", 12, "Blue");
        var handler = new CreateRentalCarRequestHandler(session);

        //Act
        var expected = await handler.Handle(request, CancellationToken.None);
        WaitForIndexing(store);
        var actual = await session.Query<RentalCar>().FirstAsync();

        //Assert
        Assert.Equal(expected.Id, actual.Id);
    }
}