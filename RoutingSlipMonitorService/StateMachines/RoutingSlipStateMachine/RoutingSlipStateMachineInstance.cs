using Automatonymous;
using MassTransit.Saga;

namespace RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;

public class RoutingSlipStateMachineInstance : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    
    public int Version { get; set; }
    public int CurrentState { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public TimeSpan? Duration { get; set; }

    public DateTime? CreateTime { get; set; }

    public string FaultSummary { get; set; }
}