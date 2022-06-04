using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using FlightService.Api;
using FlightService.Api.Controllers.v1;
using FlightService.Contracts.Airplane;
using FlightService.Contracts.Flight;
using FlightService.Domain;
using FlightService.Infrastructure.Requests.CreateAirplane;
using FlightService.Infrastructure.Requests.CreateFlight;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
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
        var postAirplaneRequest = new PostAirplaneRequest()
        {
            ModelNumber = Guid.NewGuid(),
            Seats = 0,
            AirlineName = string.Empty,
            AirplaneMakerName = string.Empty
        };
        
        var airplane = new Airplane()
        {
            Id = Guid.NewGuid().ToString(),
            Seats = new [] { new Seat {Id = Guid.NewGuid().ToString()} },
            AirlineName = string.Empty,
            ModelNumber = Guid.NewGuid(),
            AirplaneMakerName = string.Empty
        };
        
        var fakeMediator = new Mock<IMediator>();
        fakeMediator.Setup(x => x.Send(It.IsAny<CreateAirplaneRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(airplane)
            .Verifiable();
        
        var controller = new AirplaneController(fakeMediator.Object);

        //Act
        var response = await controller.PostAsync(postAirplaneRequest);

        //Assert
        fakeMediator.Verify();
        Assert.True(response is CreatedResult);
    }
}