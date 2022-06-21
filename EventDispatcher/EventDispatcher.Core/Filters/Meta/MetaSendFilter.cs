using MassTransit;
using Meta;

namespace EventDispatcher.Filters.Meta;

public class MetaSendFilter<T> : IFilter<SendContext<T>> where T : class
{
    private readonly IMetaContextAccessor _metaContextAccessor;

    public MetaSendFilter(IMetaContextAccessor metaContextAccessor)
    {
        _metaContextAccessor = metaContextAccessor;
    }

    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        _metaContextAccessor.MetaContext ??= new MetaContext();
        context.CorrelationId = _metaContextAccessor.MetaContext.CorrelationId;
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("MetaSendFilter");
    }
}