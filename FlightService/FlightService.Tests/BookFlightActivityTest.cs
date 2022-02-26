using System;
using System.Threading;
using System.Threading.Tasks;
using EventDispatcher.Test;
using FlightService.Contracts.BookFlightActivity;
using FlightService.Infrastructure.CourierActivities;
using FlightService.Infrastructure.Requests;
using FlightService.Infrastructure.Requests.CreateBookFlight;
using FlightService.Infrastructure.Requests.DeleteBookFlight;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MassTransit.Testing;
using MediatR;
using Moq;
using Xunit;

namespace FlightService.Tests;

public class BookFlightActivityTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Execute_ExpectCompleted()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        var harness = new InMemoryTestHarness();
        var rentCarActivity = new BookFlightActivity(fakeMediator.Object);
        var bookFlightActivityHarness =
            harness.Activity<BookFlightActivity, BookFlightArgument, BookFlightLog>(
                _ => rentCarActivity,
                _ => rentCarActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookFlightRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new BookFlightArgument
            {
                FlightId = Guid.NewGuid(),
                SeatId = 1
            };
            builder.AddActivity(bookFlightActivityHarness.Name, bookFlightActivityHarness.ExecuteAddress, argument);
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
        var rentCarActivity = new BookFlightActivity(fakeMediator.Object);
        var bookFlightActivityHarness =
            harness.Activity<BookFlightActivity, BookFlightArgument, BookFlightLog>(
                _ => rentCarActivity,
                _ => rentCarActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookFlightRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Error)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new BookFlightArgument
            {
                FlightId = Guid.NewGuid(),
                SeatId = 1
            };
            builder.AddActivity(bookFlightActivityHarness.Name, bookFlightActivityHarness.ExecuteAddress, argument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityFaulted>();
            var completedContext = harness.SubscribeHandler<RoutingSlipFaulted>();
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
    public async Task Compensate_ExpectCompensated()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        var harness = new InMemoryTestHarness();
        var rentCarActivity = new BookFlightActivity(fakeMediator.Object);
        var bookFlightActivityHarness =
            harness.Activity<BookFlightActivity, BookFlightArgument, BookFlightLog>(
                _ => rentCarActivity,
                _ => rentCarActivity);
        var testActivityHarness =
            harness.Activity<TestActivity, TestArgument, TestLog>();
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookFlightRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        fakeMediator.Setup(m => m.Send(It.IsAny<DeleteBookFlightRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var rentCarArgument = new BookFlightArgument
            {
                FlightId = Guid.NewGuid(),
                SeatId = 10
            };
            var testArgument = new TestArgument
            {
                IsExecuteFaulty = true
            };
            builder.AddActivity(bookFlightActivityHarness.Name, bookFlightActivityHarness.ExecuteAddress,
                rentCarArgument);
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
        var rentCarActivity = new BookFlightActivity(fakeMediator.Object);
        var bookFlightActivityHarness =
            harness.Activity<BookFlightActivity, BookFlightArgument, BookFlightLog>(
                _ => rentCarActivity,
                _ => rentCarActivity);
        var testActivityHarness =
            harness.Activity<TestActivity, TestArgument, TestLog>();
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookFlightRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        fakeMediator.Setup(m => m.Send(It.IsAny<DeleteBookFlightRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Error)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var rentCarArgument = new BookFlightArgument
            {
                FlightId = Guid.NewGuid(),
                SeatId = 10
            };
            var testArgument = new TestArgument
            {
                IsExecuteFaulty = true
            };
            builder.AddActivity(bookFlightActivityHarness.Name, bookFlightActivityHarness.ExecuteAddress,
                rentCarArgument);
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