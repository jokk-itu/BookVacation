using MassTransit;
using MassTransit.Courier.Contracts;
using MediatR;
using TrackingService.Infrastructure.Requests.UpdateTracking;

namespace TrackingService.Infrastructure.Consumers;

public class RoutingSlipEventConsumer :
    IConsumer<RoutingSlipCompleted>,
    IConsumer<RoutingSlipFaulted>,
    IConsumer<RoutingSlipCompensationFailed>,
    IConsumer<RoutingSlipActivityCompensated>,
    IConsumer<RoutingSlipActivityCompleted>,
    IConsumer<RoutingSlipActivityFaulted>,
    IConsumer<RoutingSlipActivityCompensationFailed>
{
    private readonly IMediator _mediator;

    public RoutingSlipEventConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<RoutingSlipActivityCompensated> context)
    {
        await UpdateTracking(context.Message.TrackingNumber.ToString(), context.Message.GetType().Name,
            context.Message.Timestamp);
    }

    public async Task Consume(ConsumeContext<RoutingSlipActivityCompensationFailed> context)
    {
        await UpdateTracking(context.Message.TrackingNumber.ToString(), context.Message.GetType().Name,
            context.Message.Timestamp);
    }

    public async Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
    {
        await UpdateTracking(context.Message.TrackingNumber.ToString(), context.Message.GetType().Name,
            context.Message.Timestamp);
    }

    public async Task Consume(ConsumeContext<RoutingSlipActivityFaulted> context)
    {
        await UpdateTracking(context.Message.TrackingNumber.ToString(), context.Message.GetType().Name,
            context.Message.Timestamp);
    }

    public async Task Consume(ConsumeContext<RoutingSlipCompensationFailed> context)
    {
        await UpdateTracking(context.Message.TrackingNumber.ToString(), context.Message.GetType().Name,
            context.Message.Timestamp);
    }

    public async Task Consume(ConsumeContext<RoutingSlipCompleted> context)
    {
        await UpdateTracking(context.Message.TrackingNumber.ToString(), context.Message.GetType().Name,
            context.Message.Timestamp);
    }

    public async Task Consume(ConsumeContext<RoutingSlipFaulted> context)
    {
        await UpdateTracking(context.Message.TrackingNumber.ToString(), context.Message.GetType().Name,
            context.Message.Timestamp);
    }

    private async Task UpdateTracking(string trackingNumber, string result, DateTimeOffset occuredAt)
    {
        await _mediator.Send(new UpdateTrackingRequest(trackingNumber, result, occuredAt));
    }
}