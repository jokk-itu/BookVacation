using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using TrackingService.Domain;

namespace TrackingService.Infrastructure.Requests.ReadTracking;

public class ReadTrackingRequestHandler : IRequestHandler<ReadTrackingRequest, Tracking?>
{
    private readonly IAsyncDocumentSession _session;

    public ReadTrackingRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<Tracking?> Handle(ReadTrackingRequest request, CancellationToken cancellationToken)
    {
        var tracking = await _session.Query<Tracking>().Where(x => x.Id == request.TrackingNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return tracking;
    }
}