using EventDispatcher.Test;
using HotelService.Contracts.HotelRoomReservationActivity;
using HotelService.Domain;
using HotelService.Infrastructure.CourierActivities;
using HotelService.Infrastructure.Requests.CreateHotelRoomReservation;
using HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MassTransit.Testing;
using MediatR;
using Moq;
using Xunit;

namespace HotelService.Tests;

public class BookHotelActivityTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Execute_ExpectCompleted()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        var harness = new InMemoryTestHarness();
        var bookHotelActivity = new HotelRoomReservationActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<HotelRoomReservationActivity, HotelRoomReservationArgument, HotelRoomReservationLog>(
                _ => bookHotelActivity,
                _ => bookHotelActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateHotelRoomReservationRequest>(), CancellationToken.None))
            .ReturnsAsync(new HotelRoomReservation
            {
                Id = It.IsAny<Guid>().ToString(),
                HotelId = It.IsAny<Guid>().ToString(),
                RoomId = It.IsAny<Guid>().ToString(),
                From = It.IsAny<DateTimeOffset>(),
                To = It.IsAny<DateTimeOffset>()
            })
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new HotelRoomReservationArgument
            {
                HotelId = It.IsAny<Guid>().ToString(),
                RoomId = It.IsAny<Guid>().ToString(),
                From = It.IsAny<DateTimeOffset>(),
                To = It.IsAny<DateTimeOffset>()
            };
            builder.AddActivity(bookHotelActivityHarness.Name, bookHotelActivityHarness.ExecuteAddress, argument);
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
        var rentCarActivity = new HotelRoomReservationActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<HotelRoomReservationActivity, HotelRoomReservationArgument, HotelRoomReservationLog>(
                _ => rentCarActivity,
                _ => rentCarActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateHotelRoomReservationRequest>(), CancellationToken.None))
            .ReturnsAsync((HotelRoomReservation?)null)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new HotelRoomReservationArgument
            {
                HotelId = It.IsAny<Guid>().ToString(),
                RoomId = It.IsAny<Guid>().ToString(),
                From = It.IsAny<DateTimeOffset>(),
                To = It.IsAny<DateTimeOffset>()
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
        var rentCarActivity = new HotelRoomReservationActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<HotelRoomReservationActivity, HotelRoomReservationArgument, HotelRoomReservationLog>(
                _ => rentCarActivity,
                _ => rentCarActivity);
        var testActivityHarness =
            harness.Activity<TestActivity, TestArgument, TestLog>();
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateHotelRoomReservationRequest>(), CancellationToken.None))
            .ReturnsAsync(new HotelRoomReservation
            {
                Id = It.IsAny<Guid>().ToString(),
                HotelId = It.IsAny<Guid>().ToString(),
                RoomId = It.IsAny<Guid>().ToString(),
                From = It.IsAny<DateTimeOffset>(),
                To = It.IsAny<DateTimeOffset>()
            })
            .Verifiable();

        fakeMediator.Setup(m => m.Send(It.IsAny<DeleteHotelRoomReservationRequest>(), CancellationToken.None))
            .ReturnsAsync(Unit.Value)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var bookHotelArgument = new HotelRoomReservationArgument
            {
                HotelId = It.IsAny<Guid>().ToString(),
                RoomId = It.IsAny<Guid>().ToString(),
                From = It.IsAny<DateTimeOffset>(),
                To = It.IsAny<DateTimeOffset>()
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