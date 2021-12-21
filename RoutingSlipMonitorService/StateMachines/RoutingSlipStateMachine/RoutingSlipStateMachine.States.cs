using Automatonymous;

namespace RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;

public partial class RoutingSlipStateMachine
{
    private State Executing { get; set; }
    private State Completed { get; set; }
    private State Faulted { get; set; }
    private State CompensationFailed { get; set; }
    
    private void SetupStates()
    {
        InstanceState(x => x.CurrentState, Executing, Completed, Faulted, CompensationFailed);
    }
}