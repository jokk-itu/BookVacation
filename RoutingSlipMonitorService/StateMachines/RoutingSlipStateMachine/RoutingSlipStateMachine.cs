using Automatonymous;

namespace RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;

public partial class RoutingSlipStateMachine : MassTransitStateMachine<RoutingSlipStateMachineInstance>
{
    private readonly ILogger<RoutingSlipStateMachine> _logger;

    public RoutingSlipStateMachine(ILogger<RoutingSlipStateMachine> logger)
    {
        _logger = logger;
        SetupStates();
        SetupEvents();
        SetupBehaviours();
    }
}