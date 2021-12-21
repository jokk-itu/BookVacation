using Automatonymous;

namespace RoutingSlipMonitorService.StateMachines.RoutingSlipStateMachine;

public partial class RoutingSlipStateMachine
{
    private void SetupBehaviours()
    {
        Initially(
            When(SlipCreated)
                .ThenAsync(context =>
                {
                    context.Instance.CreateTime = context.Data.Timestamp;
                    return Task.CompletedTask;
                })
                .TransitionTo(Executing),
            When(SlipCompleted)
                .ThenAsync(context =>
                {
                    context.Instance.EndTime = context.Data.Timestamp;
                    context.Instance.Duration = context.Data.Duration;
                    return Task.CompletedTask;
                })
                .TransitionTo(Completed),
            When(SlipFaulted)
                .ThenAsync(context =>
                {
                    context.Instance.EndTime = context.Data.Timestamp;
                    context.Instance.Duration = context.Data.Duration;
                    return Task.CompletedTask;
                })
                .TransitionTo(Faulted),
            When(SlipCompensationFailed)
                .TransitionTo(CompensationFailed));
        
        DuringAny(
            When(SlipCreated)
                .ThenAsync(context =>
                {
                    context.Instance.CreateTime = context.Data.Timestamp;
                    return Task.CompletedTask;
                }),
            When(SlipCompleted)
                .ThenAsync(context =>
                {
                    context.Instance.EndTime = context.Data.Timestamp;
                    context.Instance.Duration = context.Data.Duration;
                    return Task.CompletedTask;
                })
                .TransitionTo(Completed),
            When(SlipFaulted)
                .ThenAsync(context =>
                {
                    context.Instance.EndTime = context.Data.Timestamp;
                    context.Instance.Duration = context.Data.Duration;
                    return Task.CompletedTask;
                })
                .TransitionTo(Faulted),
            When(SlipCompensationFailed)
                .TransitionTo(CompensationFailed));
    }
}