using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using TrackingService.Domain;

namespace TrackingService.Infrastructure.Requests.UpdateTracking;

public class UpdateTrackingRequestHandler : IRequestHandler<UpdateTrackingRequest, Tracking>
{
    private readonly IAsyncDocumentSession _session;

    public UpdateTrackingRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<Tracking> Handle(UpdateTrackingRequest request, CancellationToken cancellationToken)
    {
        var tracking =
            await _session.Query<Tracking>().Where(x => x.Id == request.TrackingNumber)
                .FirstOrDefaultAsync(cancellationToken);

        if (tracking is null)
        {
            tracking = new Tracking
            {
                Id = request.TrackingNumber,
                Statuses = new List<Status>()
            };
            await _session.StoreAsync(tracking, cancellationToken);
        }

        tracking.Statuses.Add(new Status
        {
            Result = request.Result,
            OccuredAt = request.OccuredAt
        });

        await _session.SaveChangesAsync(cancellationToken);

        return tracking;
    }
}