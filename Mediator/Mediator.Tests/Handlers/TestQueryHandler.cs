namespace Mediator.Tests.Handlers;

public class TestQueryHandler : IQueryHandler<TestQuery, int>
{
    public async Task<Response<int>> Handle(TestQuery query, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return new Response<int>(1);
    }
}