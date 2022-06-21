using MassTransit;
using Meta;

namespace EventDispatcher.Filters.Meta;

public class MetaCompensateFilter<T> : IFilter<CompensateContext<T>> where T : class
{
    private readonly IMetaContextAccessor _metaContextAccessor;

    public MetaCompensateFilter(IMetaContextAccessor metaContextAccessor)
    {
        _metaContextAccessor = metaContextAccessor;
    }

    public async Task Send(CompensateContext<T> context, IPipe<CompensateContext<T>> next)
    {
        _metaContextAccessor.MetaContext ??= new MetaContext();
        _metaContextAccessor.MetaContext.CorrelationId = context.CorrelationId ?? Guid.NewGuid();
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("MetaCompensateFilter");
    }
}