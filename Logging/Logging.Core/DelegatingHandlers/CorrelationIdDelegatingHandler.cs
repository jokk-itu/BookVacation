using Logging.Constants;
using Meta;

namespace Logging.DelegatingHandlers;

public class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private readonly IMetaContextAccessor _metaContextAccessor;

    public CorrelationIdDelegatingHandler(IMetaContextAccessor metaContextAccessor)
    {
        _metaContextAccessor = metaContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _metaContextAccessor.MetaContext ??= new MetaContext();

        request.Headers.Add(Header.CorrelationId, _metaContextAccessor.MetaContext.CorrelationId.ToString());
        return await base.SendAsync(request, cancellationToken);
    }
}