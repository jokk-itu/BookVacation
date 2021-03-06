using CarService.Contracts.RentalDeal;
using CarService.Domain;
using CarService.Infrastructure.CourierActivities;
using CarService.Infrastructure.Requests.CreateRentalDeal;
using CarService.Infrastructure.Requests.DeleteRentalDeal;
using EventDispatcher.Test;
using MassTransit;
using MassTransit.Courier.Contracts;
using MassTransit.Testing;
using Mediator;
using MediatR;
using Moq;
using Xunit;

namespace CarService.Tests;

public class RentalDealActivityTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Execute_ExpectCompleted()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        var harness = new InMemoryTestHarness();
        var rentalDealActivity = new RentalDealActivity(fakeMediator.Object);
        var rentalDealHarness =
            harness.Activity<RentalDealActivity, RentalDealArgument, RentalDealLog>(_ => rentalDealActivity,
                _ => rentalDealActivity);
        var rentalDeal = new RentalDeal
        {
            Id = It.IsAny<Guid>()
                .ToString(),
            RentFrom = It.IsAny<DateTimeOffset>(),
            RentTo = It.IsAny<DateTimeOffset>(),
            RentalCarId = It.IsAny<Guid>()
        };
        var rentalDealResponse = new Mediator.Response<RentalDeal>(rentalDeal);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentalDealCommand>(), CancellationToken.None))
            .ReturnsAsync(rentalDealResponse)
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new RentalDealArgument
            {
                RentFrom = It.IsAny<DateTimeOffset>(),
                RentTo = It.IsAny<DateTimeOffset>(),
                RentalCarId = It.IsAny<Guid>()
            };
            builder.AddActivity(rentalDealHarness.Name, rentalDealHarness.ExecuteAddress, argument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityCompleted>();
            var completedContext = harness.SubscribeHandler<RoutingSlipCompleted>();
            await harness.Bus.Execute(builder.Build());
            await Task.WhenAll(activityContext, completedContext);

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
        var rentCarActivity = new RentalDealActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<RentalDealActivity, RentalDealArgument, RentalDealLog>(_ => rentCarActivity,
                _ => rentCarActivity);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentalDealCommand>(), CancellationToken.None))
            .ReturnsAsync(new Mediator.Response<RentalDeal> { ResponseCode = ResponseCode.NotFound })
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var argument = new RentalDealArgument
            {
                RentFrom = It.IsAny<DateTimeOffset>(),
                RentTo = It.IsAny<DateTimeOffset>(),
                RentalCarId = It.IsAny<Guid>()
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
        var rentCarActivity = new RentalDealActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<RentalDealActivity, RentalDealArgument, RentalDealLog>(_ => rentCarActivity,
                _ => rentCarActivity);
        var testActivityHarness =
            harness.Activity<TestActivity, TestArgument, TestLog>();
        var rentalDeal = new RentalDeal
        {
            Id = It.IsAny<Guid>()
                .ToString(),
            RentFrom = It.IsAny<DateTimeOffset>(),
            RentTo = It.IsAny<DateTimeOffset>(),
            RentalCarId = It.IsAny<Guid>()
        };
        var rentalDealResponse = new Mediator.Response<RentalDeal>(rentalDeal);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentalDealCommand>(), CancellationToken.None))
            .ReturnsAsync(rentalDealResponse)
            .Verifiable();

        fakeMediator.Setup(m => m.Send(It.IsAny<DeleteRentalDealCommand>(), CancellationToken.None))
            .ReturnsAsync(new Mediator.Response<Unit>())
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var bookHotelArgument = new RentalDealArgument
            {
                RentFrom = It.IsAny<DateTimeOffset>(),
                RentTo = It.IsAny<DateTimeOffset>(),
                RentalCarId = It.IsAny<Guid>()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Compensate_ExpectFailed()
    {
        //Arrange
        var fakeMediator = new Mock<IMediator>();
        var harness = new InMemoryTestHarness();
        var rentCarActivity = new RentalDealActivity(fakeMediator.Object);
        var bookHotelActivityHarness =
            harness.Activity<RentalDealActivity, RentalDealArgument, RentalDealLog>(_ => rentCarActivity,
                _ => rentCarActivity);
        var testActivityHarness =
            harness.Activity<TestActivity, TestArgument, TestLog>();
        var rentalDeal = new RentalDeal
        {
            Id = It.IsAny<Guid>()
                .ToString(),
            RentFrom = It.IsAny<DateTimeOffset>(),
            RentTo = It.IsAny<DateTimeOffset>(),
            RentalCarId = It.IsAny<Guid>()
        };
        var rentalDealResponse = new Mediator.Response<RentalDeal>(rentalDeal);
        fakeMediator.Setup(m => m.Send(It.IsAny<CreateRentalDealCommand>(), CancellationToken.None))
            .ReturnsAsync(rentalDealResponse)
            .Verifiable();

        fakeMediator.Setup(m => m.Send(It.IsAny<DeleteRentalDealCommand>(), CancellationToken.None))
            .ReturnsAsync(new Mediator.Response<Unit>
            {
                ResponseCode = ResponseCode.NotFound
            })
            .Verifiable();

        //Act
        await harness.Start();
        try
        {
            var trackingNumber = Guid.NewGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            var bookHotelArgument = new RentalDealArgument
            {
                RentFrom = It.IsAny<DateTimeOffset>(),
                RentTo = It.IsAny<DateTimeOffset>(),
                RentalCarId = It.IsAny<Guid>()
            };
            var testArgument = new TestArgument
            {
                IsExecuteFaulty = true
            };
            builder.AddActivity(bookHotelActivityHarness.Name, bookHotelActivityHarness.ExecuteAddress,
                bookHotelArgument);
            builder.AddActivity(testActivityHarness.Name, testActivityHarness.ExecuteAddress, testArgument);
            builder.AddSubscription(harness.Bus.Address, RoutingSlipEvents.All);
            var activityContext = harness.SubscribeHandler<RoutingSlipActivityCompensationFailed>();
            var faultedContext = harness.SubscribeHandler<RoutingSlipCompensationFailed>();
            await harness.Bus.Execute(builder.Build());
            await Task.WhenAll(activityContext, faultedContext);

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