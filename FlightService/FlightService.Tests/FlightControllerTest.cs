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
using FlightService.Infrastructure.Requests.CreateFlight;
using Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Xunit;

namespace FlightService.Tests;

public class FlightControllerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Post_ExpectCreated()
    {
        //Arrange
        var postFlightRequest = new PostFlightRequest
        {
            AirplaneId = Guid.NewGuid(),
            Price = 0,
            From = DateTimeOffset.Now,
            To = DateTimeOffset.Now,
            FromAirport = string.Empty,
            ToAirport = string.Empty
        };
        
        var flight = new Flight
        {
            Id = Guid.NewGuid().ToString(),
            AirPlaneId = Guid.NewGuid(),
            From = DateTimeOffset.Now,
            To = DateTimeOffset.Now,
            Price = 0,
            FromAirport = string.Empty,
            ToAirport = string.Empty
        };
        
        var fakeMediator = new Mock<IMediator>();
        fakeMediator.Setup(x => x.Send(It.IsAny<CreateFlightCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Response<Flight>(flight))
            .Verifiable();
        
        var controller = new FlightController(fakeMediator.Object);

        //Act
        var response = await controller.PostAsync(postFlightRequest);

        //Assert
        fakeMediator.Verify();
        Assert.True(response is CreatedResult);
    }
    
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Post_ExpectConflict()
    {
        //Arrange
        var postFlightRequest = new PostFlightRequest
        {
            AirplaneId = Guid.NewGuid(),
            Price = 0,
            From = DateTimeOffset.Now,
            To = DateTimeOffset.Now,
            FromAirport = string.Empty,
            ToAirport = string.Empty
        };

        var fakeMediator = new Mock<IMediator>();
        fakeMediator.Setup(x => x.Send(It.IsAny<CreateFlightCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Response<Flight>(ResponseCode.Conflict, new []{ "Flight already exists." }))
            .Verifiable();
        
        var controller = new FlightController(fakeMediator.Object);

        //Act
        var response = await controller.PostAsync(postFlightRequest);

        //Assert
        fakeMediator.Verify();
        Assert.True(response is ConflictResult);
    }
    
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Post_ExpectNotFound()
    {
        //Arrange
        var postFlightRequest = new PostFlightRequest
        {
            AirplaneId = Guid.NewGuid(),
            Price = 0,
            From = DateTimeOffset.Now,
            To = DateTimeOffset.Now,
            FromAirport = string.Empty,
            ToAirport = string.Empty
        };

        var fakeMediator = new Mock<IMediator>();
        fakeMediator.Setup(x => x.Send(It.IsAny<CreateFlightCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Response<Flight>(ResponseCode.NotFound, new [] { "Flight does not exist." }))
            .Verifiable();
        
        var controller = new FlightController(fakeMediator.Object);

        //Act
        var response = await controller.PostAsync(postFlightRequest);

        //Assert
        fakeMediator.Verify();
        Assert.True(response is NotFoundResult);
    }
}