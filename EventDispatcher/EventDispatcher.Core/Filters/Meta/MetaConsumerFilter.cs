using MassTransit;
using Meta;

namespace EventDispatcher.Filters.Meta;

public class MetaConsumerFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly IMetaContextAccessor _metaContextAccessor;

    public MetaConsumerFilter(IMetaContextAccessor metaContextAccessor)
    {
        _metaContextAccessor = metaContextAccessor;
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        _metaContextAccessor.MetaContext ??= new MetaContext();
        _metaContextAccessor.MetaContext.CorrelationId = context.CorrelationId ?? Guid.NewGuid();
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("MetaConsumeFilter");
    }
}