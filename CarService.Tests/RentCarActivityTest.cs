using System;
using System.Threading;
using System.Threading.Tasks;
using CarService.Contracts.RentCarActivity;
using CarService.Infrastructure.CourierActivities;
using CarService.Infrastructure.Requests;
using CarService.Infrastructure.Requests.CreateRentCar;
using CarService.Infrastructure.Requests.DeleteRentCar;
using EventBusTransmitting.Test;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MassTransit.Testing;
using MediatR;
using Moq;
using Xunit;

namespace CarService.Tests;

public class RentCarActivityTest
{
    private readonly Mock<IMediator> _fakeMediator;
    private readonly InMemoryTestHarness _harness;
    private readonly ActivityTestHarness<RentCarActivity, RentCarArgument, RentCarLog> _rentCarActivityHarness;
    private readonly ActivityTestHarness<TestActivity, TestArgument, TestLog> _testActivityHarness;

    public RentCarActivityTest()
    {
        _fakeMediator = new Mock<IMediator>();
        _harness = new InMemoryTestHarness();
        var rentCarActivity = new RentCarActivity(_fakeMediator.Object);
        _rentCarActivityHarness =
            _harness.Activity<RentCarActivity, RentCarArgument, RentCarLog>(_ => rentCarActivity, _ => rentCarActivity);
        _testActivityHarness =
            _harness.Activity<TestActivity, TestArgument, TestLog>();
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Execute_ExpectCompleted()
    {
        //Arrange
        _fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        //Act
        await _harness.Start();
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
            builder.AddActivity(_rentCarActivityHarness.Name, _rentCarActivityHarness.ExecuteAddress, argument);
            builder.AddSubscription(_harness.Bus.Address, RoutingSlipEvents.All);
            await _harness.Bus.Execute(builder.Build());
            var activityContext = _harness.SubscribeHandler<RoutingSlipActivityCompleted>();
            var completedContext = _harness.SubscribeHandler<RoutingSlipCompleted>();
            Task.WaitAll(activityContext, completedContext);

            //Assert
            _fakeMediator.Verify();
            Assert.Equal(trackingNumber, completedContext.Result.Message.TrackingNumber);
            Assert.Equal(trackingNumber, activityContext.Result.Message.TrackingNumber);
        }
        finally
        {
            await _harness.Stop();
        }
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Execute_ExpectFaulted()
    {
        //Arrange
        _fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Error)
            .Verifiable();

        //Act
        await _harness.Start();
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
            builder.AddActivity(_rentCarActivityHarness.Name, _rentCarActivityHarness.ExecuteAddress, argument);
            builder.AddSubscription(_harness.Bus.Address, RoutingSlipEvents.All);
            await _harness.Bus.Execute(builder.Build());
            var activityContext = _harness.SubscribeHandler<RoutingSlipActivityFaulted>();
            var faultedContext = _harness.SubscribeHandler<RoutingSlipFaulted>();
            Task.WaitAll(activityContext, faultedContext);

            //Assert
            _fakeMediator.Verify();
            Assert.Equal(trackingNumber, faultedContext.Result.Message.TrackingNumber);
            Assert.Equal(trackingNumber, activityContext.Result.Message.TrackingNumber);
        }
        finally
        {
            await _harness.Stop();
        }
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Compensate_ExpectCompensated()
    {
        //Arrange
        _fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        _fakeMediator.Setup(m => m.Send(It.IsAny<DeleteRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok).Verifiable();
        
        //Act
        await _harness.Start();
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
            builder.AddActivity(_rentCarActivityHarness.Name, _rentCarActivityHarness.ExecuteAddress, rentCarArgument);
            builder.AddActivity(_testActivityHarness.Name, _testActivityHarness.ExecuteAddress, testArgument);
            builder.AddSubscription(_harness.Bus.Address, RoutingSlipEvents.All);
            await _harness.Bus.Execute(builder.Build());
            var activityContext = _harness.SubscribeHandler<RoutingSlipActivityCompensated>();
            var faultedContext = _harness.SubscribeHandler<RoutingSlipFaulted>();
            Task.WaitAll(activityContext, faultedContext);

            //Assert
            _fakeMediator.Verify();
            Assert.Equal(trackingNumber, faultedContext.Result.Message.TrackingNumber);
            Assert.Equal(trackingNumber, activityContext.Result.Message.TrackingNumber);
        }
        finally
        {
            await _harness.Stop();
        }
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Compensate_ExpectCompensationFailed()
    {
        //Arrange
        _fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        _fakeMediator.Setup(m => m.Send(It.IsAny<DeleteRentCarRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Error)
            .Verifiable();
        
        //Act
        await _harness.Start();
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
            builder.AddActivity(_rentCarActivityHarness.Name, _rentCarActivityHarness.ExecuteAddress, rentCarArgument);
            builder.AddActivity(_testActivityHarness.Name, _testActivityHarness.ExecuteAddress, testArgument);
            builder.AddSubscription(_harness.Bus.Address, RoutingSlipEvents.All);
            await _harness.Bus.Execute(builder.Build());
            var activityContext = _harness.SubscribeHandler<RoutingSlipActivityCompensationFailed>();
            var faultedContext = _harness.SubscribeHandler<RoutingSlipCompensationFailed>();
            Task.WaitAll(activityContext, faultedContext);

            //Assert
            _fakeMediator.Verify();
            Assert.Equal(trackingNumber, faultedContext.Result.Message.TrackingNumber);
            Assert.Equal(trackingNumber, activityContext.Result.Message.TrackingNumber);
        }
        finally
        {
            await _harness.Stop();
        }
    }
}