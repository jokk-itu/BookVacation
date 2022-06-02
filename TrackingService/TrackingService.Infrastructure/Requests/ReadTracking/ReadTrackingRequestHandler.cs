using DocumentClient;
using MediatR;
using Raven.Client.Documents;
using TrackingService.Domain;

namespace TrackingService.Infrastructure.Requests.ReadTracking;

public class ReadTrackingRequestHandler : IRequestHandler<ReadTrackingRequest, Tracking?>
{
    private readonly IDocumentClient _client;

    public ReadTrackingRequestHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Tracking?> Handle(ReadTrackingRequest request, CancellationToken cancellationToken)
    {
        return await _client.QueryAsync<Tracking>(async query => await query
            .Where(x => x.Id == request.TrackingNumber)
            .FirstOrDefaultAsync(cancellationToken));
    }
}