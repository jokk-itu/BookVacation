using MassTransit;
using Meta;

namespace EventDispatcher.Filters;

public class MetaPublishFilter<T> : IFilter<PublishContext<T>> where T : class
{
    private readonly IMetaContextAccessor _metaContextAccessor;

    public MetaPublishFilter(IMetaContextAccessor metaContextAccessor)
    {
        _metaContextAccessor = metaContextAccessor;
    }
    
    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        _metaContextAccessor.MetaContext ??= new MetaContext();
        context.CorrelationId = _metaContextAccessor.MetaContext.CorrelationId;
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("MetaPublishFilter");
    }
}