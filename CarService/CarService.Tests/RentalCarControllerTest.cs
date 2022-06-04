using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Net.Http.Json;
using CarService.Api;
using CarService.Api.Controllers.v1;
using CarService.Contracts.RentalCar;
using CarService.Domain;
using CarService.Infrastructure.Requests.CreateRentalCar;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
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
        
        var fakeMediator = new Mock<IMediator>();
        fakeMediator.Setup(x => x.Send(It.IsAny<CreateRentalCarRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalCar)
            .Verifiable();
        
        var controller = new RentalCarController(fakeMediator.Object);

        //Act
        var response = await controller.PostAsync(postRentalCarRequest);

        //Assert
        fakeMediator.Verify();
        Assert.True(response is CreatedResult);
    }
}