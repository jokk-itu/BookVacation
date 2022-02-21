using MassTransit.Courier;

namespace EventBusTransmitting.Test;

public class TestActivity : IActivity<TestArgument, TestLog>
{
    public async Task<ExecutionResult> Execute(ExecuteContext<TestArgument> context)
    {
        return await Task.Run(() =>
            context.Arguments.IsExecuteFaulty
                ? context.Faulted()
                : context.Completed(new TestLog { IsCompensateFaulty = context.Arguments.IsCompensateFaulty }));
    }

    public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
    {
        return await Task.Run(() =>
            context.Log.IsCompensateFaulty ? context.Failed() : context.Compensated());
    }
}