using System;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Api.Controllers.v1;
using FlightService.Contracts.Airplane;
using FlightService.Domain;
using FlightService.Infrastructure.Requests.CreateAirplane;
using Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlightService.Tests;

public class AirplaneControllerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Post_ExpectCreated()
    {
        //Arrange
        var postAirplaneRequest = new PostAirplaneRequest
        {
            ModelNumber = Guid.NewGuid(),
            Seats = 0,
            AirlineName = string.Empty,
            AirplaneMakerName = string.Empty
        };

        var airplane = new Airplane
        {
            Id = Guid.NewGuid().ToString(),
            Seats = new[] { new Seat { Id = Guid.NewGuid().ToString() } },
            AirlineName = string.Empty,
            ModelNumber = Guid.NewGuid(),
            AirplaneMakerName = string.Empty
        };

        var fakeMediator = new Mock<IMediator>();
        fakeMediator.Setup(x => x.Send(It.IsAny<CreateAirplaneCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Response<Airplane>(airplane))
            .Verifiable();

        var controller = new AirplaneController(fakeMediator.Object);

        //Act
        var response = await controller.PostAsync(postAirplaneRequest);

        //Assert
        fakeMediator.Verify();
        Assert.True(response is CreatedResult);
    }
}