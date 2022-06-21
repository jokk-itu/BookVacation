using CarService.Api.Controllers.v1;
using CarService.Contracts.RentalCar;
using CarService.Domain;
using CarService.Infrastructure.Requests.CreateRentalCar;
using Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CarService.Tests;

public class RentalCarControllerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Post_ExpectCreated()
    {
        //Arrange
        var postRentalCarRequest = new PostRentalCarRequest
        {
            CarModelNumber = Guid.NewGuid(),
            CarCompanyName = string.Empty,
            Color = string.Empty,
            DayPrice = 0,
            RentingCompanyName = string.Empty
        };

        var rentalCar = new RentalCar
        {
            Id = Guid.NewGuid().ToString(),
            Color = string.Empty,
            DayPrice = 0,
            CarCompanyName = string.Empty,
            CarModelNumber = Guid.NewGuid(),
            RentalCompanyName = string.Empty
        };

        var rentalCarResponse = new Response<RentalCar>(rentalCar);

        var fakeMediator = new Mock<IMediator>();
        fakeMediator.Setup(x => x.Send(It.IsAny<CreateRentalCarCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalCarResponse)
            .Verifiable();

        var controller = new RentalCarController(fakeMediator.Object);

        //Act
        var response = await controller.PostAsync(postRentalCarRequest);

        //Assert
        fakeMediator.Verify();
        Assert.True(response is CreatedResult);
    }
}