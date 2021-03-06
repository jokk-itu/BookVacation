using CarService.Domain;
using CarService.Infrastructure.Requests.CreateRentalCar;
using Mediator;
using Microsoft.Extensions.Logging;
using Moq;
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

        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var request = new CreateRentalCarCommand(Guid.NewGuid(), "Mercedes", "EuropeCar", 12, "Blue");
        var handler = new CreateRentalCarCommandHandler(client);

        //Act
        var response = await handler.Handle(request, CancellationToken.None);
        WaitForIndexing(store);
        var actual = await session.Query<RentalCar>().FirstAsync();

        //Assert
        Assert.Equal(ResponseCode.Ok, response.ResponseCode);
        Assert.Equal(response.Body!.Id, actual.Id);
    }
}