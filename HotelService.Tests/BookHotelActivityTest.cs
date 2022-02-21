using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts.BookHotelActivity;
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
    private readonly Mock<IMediator> _fakeMediator;
    private readonly InMemoryTestHarness _harness;
    private readonly ActivityTestHarness<BookHotelActivity, BookHotelArgument, BookHotelLog> _bookHotelActivityHarness;
    private readonly ActivityTestHarness<TestActivity, TestArgument, TestLog> _testActivityHarness;
    
    public BookHotelActivityTest()
    {
        _fakeMediator = new Mock<IMediator>();
        _harness = new InMemoryTestHarness();
        var rentCarActivity = new BookHotelActivity(_fakeMediator.Object);
        _bookHotelActivityHarness =
            _harness.Activity<BookHotelActivity, BookHotelArgument, BookHotelLog>(_ => rentCarActivity, _ => rentCarActivity);
        _testActivityHarness =
            _harness.Activity<TestActivity, TestArgument, TestLog>();
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Execute_ExpectCompleted()
    {
        //Arrange
        _fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        //Act
        await _harness.Start();
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
            builder.AddActivity(_bookHotelActivityHarness.Name, _bookHotelActivityHarness.ExecuteAddress, argument);
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
        _fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Error)
            .Verifiable();

        //Act
        await _harness.Start();
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
            builder.AddActivity(_bookHotelActivityHarness.Name, _bookHotelActivityHarness.ExecuteAddress, argument);
            builder.AddSubscription(_harness.Bus.Address, RoutingSlipEvents.All);
            await _harness.Bus.Execute(builder.Build());
            var activityContext = _harness.SubscribeHandler<RoutingSlipActivityFaulted>();
            var completedContext = _harness.SubscribeHandler<RoutingSlipFaulted>();
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
    public async Task Compensate_ExpectCompensated()
    {
        //Arrange
        _fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        _fakeMediator.Setup(m => m.Send(It.IsAny<DeleteBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();
        
        //Act
        await _harness.Start();
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
            builder.AddActivity(_bookHotelActivityHarness.Name, _bookHotelActivityHarness.ExecuteAddress, bookHotelArgument);
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
        _fakeMediator.Setup(m => m.Send(It.IsAny<CreateBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Ok)
            .Verifiable();

        _fakeMediator.Setup(m => m.Send(It.IsAny<DeleteBookHotelRequest>(), CancellationToken.None))
            .ReturnsAsync(RequestResult.Error)
            .Verifiable();
        
        //Act
        await _harness.Start();
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
            builder.AddActivity(_bookHotelActivityHarness.Name, _bookHotelActivityHarness.ExecuteAddress, bookHotelArgument);
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