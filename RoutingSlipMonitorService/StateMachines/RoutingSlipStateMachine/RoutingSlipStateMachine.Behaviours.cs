using Automatonymous;

namespace RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;

public partial class RoutingSlipStateMachine
{
    private void SetupBehaviours()
    {
        Initially(When(ActivityCompleted).TransitionTo(Executing), 
            When(ActivityFaulted).TransitionTo(FaultyStart), 
            Ignore(Faulted), 
            Ignore(Completed), 
            Ignore(CompensationFailed), 
            Ignore(ActivityCompensated), 
            Ignore(ActivityCompensationFailed));
        
        During(FaultyStart, When(Faulted).Finalize(),
            Ignore(ActivityFaulted));
        
        During(Executing, When(ActivityCompleted).TransitionTo(Executing), 
            When(ActivityFaulted).TransitionTo(Compensating), 
            When(Completed).Finalize(), 
            Ignore(Faulted), 
            Ignore(ActivityCompensated), 
            Ignore(ActivityCompensationFailed), 
            Ignore(CompensationFailed));
        
        During(Compensating, When(ActivityCompensated).TransitionTo(Compensating), 
            When(Faulted).Finalize(), 
            When(ActivityCompensationFailed).TransitionTo(FaultyCompensation), 
            Ignore(ActivityFaulted), 
            Ignore(CompensationFailed));
        
        During(FaultyCompensation, When(CompensationFailed).Finalize(),
            Ignore(ActivityCompensationFailed),
            Ignore(Faulted),
            Ignore(Completed),
            Ignore(ActivityCompensated),
            Ignore(ActivityFaulted),
            Ignore(ActivityCompleted));
        
        SetCompletedWhenFinalized();
    }
}