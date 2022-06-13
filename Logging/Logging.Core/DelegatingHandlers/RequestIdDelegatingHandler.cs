using Logging.Constants;

namespace Logging.DelegatingHandlers;

public class RequestIdDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add(Header.RequestId, Guid.NewGuid().ToString());
        return await base.SendAsync(request, cancellationToken);
    }
}