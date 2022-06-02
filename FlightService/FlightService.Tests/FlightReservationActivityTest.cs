using System;
using System.Threading;
using System.Threading.Tasks;
using EventDispatcher.Test;
using FlightService.Contracts.FlightReservation;
using FlightService.Domain;
using FlightService.Infrastructure.CourierActivities;
using FlightService.Infrastructure.Requests.CreateFlightReservation;
using FlightService.Infrastructure.Requests.DeleteFlightReservation;
using MassTransit;
using MassTransit.Courier.Contracts;
using MassTransit.Testing;
using MediatR;
using Moq;
using Xunit;

namespace FlightService.Tests;

public class FlightReservationActivityTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Execute_ExpectCompleted()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        var harness = new InMemoryTestHarness();
        var flightReservationActivity = new FlightReservationActivity(fakeMediator.Object);
        var flightReservationHarness =
            harness.Activity<FlightReservationActivity, FlightReservationArgument, FlightReservationLog>(
                _ => flightReservationActivity,
                _ => flightReservationActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateFlightReservationRequest>(), CancellationToken.None))
            .ReturnsAsync(new FlightReservation
            {
                Id = It.IsAny<Guid>().ToString(),
                FlightId = It.IsAny<Guid>(),
                SeatId = It.IsAny<Guid>()
            })
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new FlightReservationArgument
            {
                FlightId = Guid.NewGuid(),
                SeatId = Guid.NewGuid()
            };
            builder.AddActivity(flightReservationHarness.Name, flightReservationHarness.ExecuteAddress, argument);
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
        var rentCarActivity = new FlightReservationActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<FlightReservationActivity, FlightReservationArgument, FlightReservationLog>(
                _ => rentCarActivity,
                _ => rentCarActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateFlightReservationRequest>(), CancellationToken.None))
            .ReturnsAsync((FlightReservation?)null)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new FlightReservationArgument
            {
                FlightId = Guid.NewGuid(),
                SeatId = Guid.NewGuid()
            };
            builder.AddActivity(bookHotelActivityHarness.Name, bookHotelActivityHarness.ExecuteAddress, argument);
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
        var rentCarActivity = new FlightReservationActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<FlightReservationActivity, FlightReservationArgument, FlightReservationLog>(
                _ => rentCarActivity,
                _ => rentCarActivity);
        var testActivityHarness =
            harness.Activity<TestActivity, TestArgument, TestLog>();
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateFlightReservationRequest>(), CancellationToken.None))
            .ReturnsAsync(new FlightReservation
            {
                Id = It.IsAny<Guid>().ToString(),
                FlightId = It.IsAny<Guid>(),
                SeatId = It.IsAny<Guid>()
            })
            .Verifiable();

        fakeMediator.Setup(m => m.Send(It.IsAny<DeleteFlightReservationRequest>(), CancellationToken.None))
            .ReturnsAsync(Unit.Value)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var bookHotelArgument = new FlightReservationArgument
            {
                FlightId = Guid.NewGuid(),
                SeatId = Guid.NewGuid()
            };
            var testArgument = new TestArgument
            {
                IsExecuteFaulty = true
            };
            builder.AddActivity(bookHotelActivityHarness.Name, bookHotelActivityHarness.ExecuteAddress,
                bookHotelArgument);
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
}