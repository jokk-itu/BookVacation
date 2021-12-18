using Automatonymous;
using MassTransit.Saga;

namespace RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;

public class RoutingSlipStateMachineInstance : SagaStateMachineInstance, ISagaVersion
{
    public Guid TrackingNumber { get; set; }
    
    public Guid CorrelationId { get; set; }
    
    public int Version { get; set; }
    
    public int CurrentState { get; set; }
}