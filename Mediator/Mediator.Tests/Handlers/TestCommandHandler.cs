namespace Mediator.Tests.Handlers;

public class TestCommandHandler : ICommandHandler<TestCommand, int>
{
    public async Task<Response<int>> Handle(TestCommand command, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return new Response<int>(1);
    }
}