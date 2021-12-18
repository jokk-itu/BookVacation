using Automatonymous;
using MassTransit.Courier.Contracts;

namespace RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;

public partial class RoutingSlipStateMachine
{
    private Event<RoutingSlipCompleted> Completed { get; init; }

    private Event<RoutingSlipFaulted> Faulted { get; init; }

    private Event<RoutingSlipCompensationFailed> CompensationFailed { get; init; }

    private Event<RoutingSlipActivityCompleted> ActivityCompleted { get; init; }

    private Event<RoutingSlipActivityFaulted> ActivityFaulted { get; init; }

    private Event<RoutingSlipActivityCompensated> ActivityCompensated { get; init; }

    private Event<RoutingSlipActivityCompensationFailed> ActivityCompensationFailed { get; init; }

    private void SetupEvents()
    {
        Event(() => Completed, configurator => configurator.CorrelateById(context => context.Message.TrackingNumber));
        Event(() => Faulted, configurator => configurator.CorrelateById(context => context.Message.TrackingNumber));
        Event(() => CompensationFailed, configurator => configurator.CorrelateById(context => context.Message.TrackingNumber));
        Event(() => ActivityCompleted, configurator => configurator.CorrelateById(context => context.Message.TrackingNumber));
        Event(() => ActivityFaulted, configurator => configurator.CorrelateById(c => c.Message.TrackingNumber));
        Event(() => ActivityCompensated, configurator => configurator.CorrelateById(context => context.Message.TrackingNumber));
        Event(() => ActivityCompensationFailed, configurator => configurator.CorrelateById(context => context.Message.TrackingNumber));
    }
}