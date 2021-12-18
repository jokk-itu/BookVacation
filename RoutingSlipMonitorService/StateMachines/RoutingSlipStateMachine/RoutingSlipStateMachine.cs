using Automatonymous;

namespace RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;

public partial class RoutingSlipStateMachine : MassTransitStateMachine<RoutingSlipStateMachineInstance>
{
    public RoutingSlipStateMachine()
    {
        SetupStates();
        SetupEvents();
        SetupBehaviours();
    }
}