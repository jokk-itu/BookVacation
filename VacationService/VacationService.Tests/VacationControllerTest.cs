using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VacationService.Api.Controllers;
using VacationService.Api.Controllers.v1;
using VacationService.Contracts.Vacation;
using VacationService.Infrastructure.Requests.CreateVacation;
using Xunit;

namespace VacationService.Test;

public class VacationControllerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task PostAsync_ExpectAccepted()
    {
        //Arrange
        var request = new PostVacationRequest();
        var fakeMediator = new Mock<IMediator>();
        fakeMediator.Setup(x => x.Send(It.IsAny<CreateVacationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value)
            .Verifiable();
        var controller = new VacationController(fakeMediator.Object);

        //Act
        var response = await controller.PostAsync(request);

        //Assert
        Assert.True(response is AcceptedResult);
    }
}