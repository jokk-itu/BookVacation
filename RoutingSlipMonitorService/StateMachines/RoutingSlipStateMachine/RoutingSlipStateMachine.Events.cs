using Automatonymous;
using Contracts;
using MassTransit.Courier.Contracts;

namespace RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;

public partial class RoutingSlipStateMachine
{
    private Event<RoutingSlipCreated> SlipCreated { get; set; }
    private Event<RoutingSlipCompleted> SlipCompleted { get; set; }
    private Event<RoutingSlipFaulted> SlipFaulted { get; set; }
    private Event<RoutingSlipCompensationFailed> SlipCompensationFailed { get; set; }

    private void SetupEvents()
    {
        Event(() => SlipCreated, x => x.CorrelateById(context => context.Message.TrackingNumber));
        Event(() => SlipCompleted, x => x.CorrelateById(context => context.Message.TrackingNumber));
        Event(() => SlipFaulted, x => x.CorrelateById(context => context.Message.TrackingNumber));
        Event(() => SlipCompensationFailed, x => x.CorrelateById(context => context.Message.TrackingNumber));
    }
}