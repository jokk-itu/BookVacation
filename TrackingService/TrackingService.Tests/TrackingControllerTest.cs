using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrackingService.Api.Controllers.v1;
using TrackingService.Domain;
using TrackingService.Infrastructure.Requests.ReadTracking;
using Xunit;

namespace TrackingService.Tests;

public class TrackingControllerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAsync_ExpectOk()
    {
        //Arrange
        var tracking = new Tracking
        {
            Id = Guid.NewGuid().ToString(),
            Statuses = new[] { new Status { Result = "Completed", OccuredAt = DateTimeOffset.Now } }
        };
        var fakeMediator = new Mock<IMediator>();
        fakeMediator.Setup(x => x.Send(It.IsAny<ReadTrackingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Response<Tracking>(tracking))
            .Verifiable();

        //Act
        var controller = new TrackingController(fakeMediator.Object);
        var response = await controller.GetAsync(Guid.NewGuid());

        //Assert
        fakeMediator.Verify();
        Assert.True(response is OkObjectResult);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAsync_ExpectNotFound()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        fakeMediator.Setup(x => x.Send(It.IsAny<ReadTrackingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Response<Tracking>(ResponseCode.NotFound, new[] { "Tracking does not exist" }))
            .Verifiable();

        //Act
        var controller = new TrackingController(fakeMediator.Object);
        var response = await controller.GetAsync(Guid.NewGuid());

        //Assert
        fakeMediator.Verify();
        Assert.True(response is NotFoundResult);
    }
}