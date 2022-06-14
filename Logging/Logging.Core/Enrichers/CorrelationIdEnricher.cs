using Meta;
using Serilog.Core;
using Serilog.Events;

namespace Logging.Enrichers;

public class CorrelationIdEnricher : ILogEventEnricher
{
    private readonly IMetaContextAccessor _metaContextAccessor;

    public CorrelationIdEnricher(IMetaContextAccessor metaContextAccessor)
    {
        _metaContextAccessor = metaContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_metaContextAccessor.MetaContext?.CorrelationId is null)
            return;

        var property = propertyFactory.CreateProperty("CorrelationId", _metaContextAccessor.MetaContext.CorrelationId);
        logEvent.AddOrUpdateProperty(property);
    }
}