using MassTransit;
using Meta;

namespace EventDispatcher.Filters.Meta;

public class MetaExecuteFilter<T> : IFilter<ExecuteContext<T>> where T : class
{
    private readonly IMetaContextAccessor _metaContextAccessor;

    public MetaExecuteFilter(IMetaContextAccessor metaContextAccessor)
    {
        _metaContextAccessor = metaContextAccessor;
    }

    public async Task Send(ExecuteContext<T> context, IPipe<ExecuteContext<T>> next)
    {
        _metaContextAccessor.MetaContext ??= new MetaContext();
        _metaContextAccessor.MetaContext.CorrelationId = context.CorrelationId ?? Guid.NewGuid();
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("MetaExecuteFilter");
    }
}