using EventBusTransmitting.Test;
using HotelService.Contracts.BookHotelActivity;
using HotelService.Infrastructure.CourierActivities;
using HotelService.Infrastructure.Requests;
using HotelService.Infrastructure.Requests.CreateBookHotel;
using HotelService.Infrastructure.Requests.DeleteBookHotel;
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
        var rentCarActivity = new BookHotelActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<BookHotelActivity, BookHotelArgument, BookHotelLog>(_ => rentCarActivity, _ => rentCarActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new BookHotelArgument
            {
                HotelId = Guid.NewGuid(),
                Days = 10u,
                RoomId = Guid.NewGuid()
            };
            builder.AddActivity(bookHotelActivityHarness.Name, bookHotelActivityHarness.ExecuteAddress, argument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            await harness.Bus.Execute(builder.Build());
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityCompleted>();
            var completedContext = harness.SubscribeHandler<RoutingSlipCompleted>();
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
        var rentCarActivity = new BookHotelActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<BookHotelActivity, BookHotelArgument, BookHotelLog>(_ => rentCarActivity, _ => rentCarActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Error)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new BookHotelArgument
            {
                HotelId = Guid.NewGuid(),
                Days = 10u,
                RoomId = Guid.NewGuid()
            };
            builder.AddActivity(bookHotelActivityHarness.Name, bookHotelActivityHarness.ExecuteAddress, argument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            await harness.Bus.Execute(builder.Build());
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityFaulted>();
            var completedContext = harness.SubscribeHandler<RoutingSlipFaulted>();
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
        var rentCarActivity = new BookHotelActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<BookHotelActivity, BookHotelArgument, BookHotelLog>(_ => rentCarActivity, _ => rentCarActivity);
        var testActivityHarness =
            harness.Activity<TestActivity, TestArgument, TestLog>();
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        fakeMediator.Setup(m => m.Send(It.IsAny<DeleteBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();
        
        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var bookHotelArgument = new BookHotelArgument
            {
                HotelId = Guid.NewGuid(),
                Days = 10u,
                RoomId = Guid.NewGuid()
            };
            var testArgument = new TestArgument
            {
                IsExecuteFaulty = true
            };
            builder.AddActivity(bookHotelActivityHarness.Name, bookHotelActivityHarness.ExecuteAddress, bookHotelArgument);
            builder.AddActivity(testActivityHarness.Name, testActivityHarness.ExecuteAddress, testArgument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            await harness.Bus.Execute(builder.Build());
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityCompensated>();
            var faultedContext = harness.SubscribeHandler<RoutingSlipFaulted>();
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
        var rentCarActivity = new BookHotelActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<BookHotelActivity, BookHotelArgument, BookHotelLog>(_ => rentCarActivity, _ => rentCarActivity);
        var testActivityHarness =
            harness.Activity<TestActivity, TestArgument, TestLog>();
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        fakeMediator.Setup(m => m.Send(It.IsAny<DeleteBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Error)
            .Verifiable();
        
        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var bookHotelArgument = new BookHotelArgument
            {
                HotelId = Guid.NewGuid(),
                Days = 10u,
                RoomId = Guid.NewGuid()
            };
            var testArgument = new TestArgument
            {
                IsExecuteFaulty = true
            };
            builder.AddActivity(bookHotelActivityHarness.Name, bookHotelActivityHarness.ExecuteAddress, bookHotelArgument);
            builder.AddActivity(testActivityHarness.Name, testActivityHarness.ExecuteAddress, testArgument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            await harness.Bus.Execute(builder.Build());
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityCompensationFailed>();
            var faultedContext = harness.SubscribeHandler<RoutingSlipCompensationFailed>();
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