using DocumentClient;
using MediatR;
using Raven.Client.Documents;
using TrackingService.Domain;

namespace TrackingService.Infrastructure.Requests.UpdateTracking;

public class UpdateTrackingRequestHandler : IRequestHandler<UpdateTrackingRequest, Tracking>
{
    private readonly IDocumentClient _client;

    public UpdateTrackingRequestHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Tracking> Handle(UpdateTrackingRequest request, CancellationToken cancellationToken)
    {
        var tracking =
            await _client.QueryAsync<Tracking>(async query => await query.Where(x => x.Id == request.TrackingNumber)
                .FirstOrDefaultAsync(cancellationToken));

        if (tracking is null)
        {
            tracking = new Tracking
            {
                Id = request.TrackingNumber,
                Statuses = new List<Status>()
            };
            await _client.StoreAsync(tracking, cancellationToken);
        }

        tracking.Statuses.Add(new Status
        {
            Result = request.Result,
            OccuredAt = request.OccuredAt
        });

        await _client.UpdateAsync(cancellationToken);

        return tracking;
    }
}