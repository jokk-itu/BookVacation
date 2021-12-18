using Automatonymous;

namespace RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;

public partial class RoutingSlipStateMachine
{
    private State FaultyStart { get; init; }

    private State Executing { get; init; }

    private State Compensating { get; init; }
    
    private State FaultyCompensation { get; init; }
    
    private void SetupStates()
    {
        InstanceState(x =>
            x.CurrentState, FaultyStart, Executing, Compensating, FaultyCompensation);
    }
}