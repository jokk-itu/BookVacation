using CarService.Contracts.RentCarActivity;
using CarService.Infrastructure.CourierActivities;
using CarService.Infrastructure.Requests;
using CarService.Infrastructure.Requests.CreateRentCar;
using CarService.Infrastructure.Requests.DeleteRentCar;
using EventDispatcher.Test;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MassTransit.Testing;
using MediatR;
using Moq;
using Xunit;

namespace CarService.Tests;

public class RentCarActivityTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Execute_ExpectCompleted()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        var harness = new InMemoryTestHarness();
        var rentCarActivity = new RentCarActivity(fakeMediator.Object);
        var rentCarActivityHarness =
            harness.Activity<RentCarActivity, RentCarArgument, RentCarLog>(_ => rentCarActivity, _ => rentCarActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new RentCarArgument
            {
                CarId = Guid.NewGuid(),
                Days = 10u,
                RentingCompanyId = Guid.NewGuid()
            };
            builder.AddActivity(rentCarActivityHarness.Name, rentCarActivityHarness.ExecuteAddress, argument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityCompleted>();
            var completedContext = harness.SubscribeHandler<RoutingSlipCompleted>();
            await harness.Bus.Execute(builder.Build());
            Task.WaitAll(activityContext, completedContext);
            
            //Assert
            fakeMediator.Verify();
            Assert.Equal(trackingNumber, completedContext.Result.Message.TrackingNumber);
            Assert.Equal(trackingNumber, activityContext.Result.Message.TrackingNumber);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Execute_ExpectFaulted()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        var harness = new InMemoryTestHarness();
        var rentCarActivity = new RentCarActivity(fakeMediator.Object);
        var rentCarActivityHarness =
            harness.Activity<RentCarActivity, RentCarArgument, RentCarLog>(_ => rentCarActivity, _ => rentCarActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Error)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new RentCarArgument
            {
                CarId = Guid.NewGuid(),
                Days = 10u,
                RentingCompanyId = Guid.NewGuid()
            };
            builder.AddActivity(rentCarActivityHarness.Name, rentCarActivityHarness.ExecuteAddress, argument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityFaulted>();
            var faultedContext = harness.SubscribeHandler<RoutingSlipFaulted>();
            await harness.Bus.Execute(builder.Build());
            Task.WaitAll(activityContext, faultedContext);

            //Assert
            fakeMediator.Verify();
            Assert.Equal(trackingNumber, faultedContext.Result.Message.TrackingNumber);
            Assert.Equal(trackingNumber, activityContext.Result.Message.TrackingNumber);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Compensate_ExpectCompensated()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        var harness = new InMemoryTestHarness();
        var rentCarActivity = new RentCarActivity(fakeMediator.Object);
        var rentCarActivityHarness =
            harness.Activity<RentCarActivity, RentCarArgument, RentCarLog>(_ => rentCarActivity, _ => rentCarActivity);
        var testActivityHarness =
            harness.Activity<TestActivity, TestArgument, TestLog>();
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        fakeMediator.Setup(m => m.Send(It.IsAny<DeleteRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok).Verifiable();
        
        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var rentCarArgument = new RentCarArgument
            {
                CarId = Guid.NewGuid(),
                Days = 10u,
                RentingCompanyId = Guid.NewGuid()
            };
            var testArgument = new TestArgument
            {
                IsExecuteFaulty = true
            };
            builder.AddActivity(rentCarActivityHarness.Name, rentCarActivityHarness.ExecuteAddress, rentCarArgument);
            builder.AddActivity(testActivityHarness.Name, testActivityHarness.ExecuteAddress, testArgument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityCompensated>();
            var faultedContext = harness.SubscribeHandler<RoutingSlipFaulted>();
            await harness.Bus.Execute(builder.Build());
            Task.WaitAll(activityContext, faultedContext);

            //Assert
            fakeMediator.Verify();
            Assert.Equal(trackingNumber, faultedContext.Result.Message.TrackingNumber);
            Assert.Equal(trackingNumber, activityContext.Result.Message.TrackingNumber);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Compensate_ExpectCompensationFailed()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        var harness = new InMemoryTestHarness();
        var rentCarActivity = new RentCarActivity(fakeMediator.Object);
        var rentCarActivityHarness =
            harness.Activity<RentCarActivity, RentCarArgument, RentCarLog>(_ => rentCarActivity, _ => rentCarActivity);
        var testActivityHarness =
            harness.Activity<TestActivity, TestArgument, TestLog>();
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        fakeMediator.Setup(m => m.Send(It.IsAny<DeleteRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Error)
            .Verifiable();
        
        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var rentCarArgument = new RentCarArgument
            {
                CarId = Guid.NewGuid(),
                Days = 10u,
                RentingCompanyId = Guid.NewGuid()
            };
            var testArgument = new TestArgument
            {
                IsExecuteFaulty = true
            };
            builder.AddActivity(rentCarActivityHarness.Name, rentCarActivityHarness.ExecuteAddress, rentCarArgument);
            builder.AddActivity(testActivityHarness.Name, testActivityHarness.ExecuteAddress, testArgument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityCompensationFailed>();
            var faultedContext = harness.SubscribeHandler<RoutingSlipCompensationFailed>();
            await harness.Bus.Execute(builder.Build());
            Task.WaitAll(activityContext, faultedContext);

            //Assert
            fakeMediator.Verify();
            Assert.Equal(trackingNumber, faultedContext.Result.Message.TrackingNumber);
            Assert.Equal(trackingNumber, activityContext.Result.Message.TrackingNumber);
        }
        finally
        {
            await harness.Stop();
        }
    }
}