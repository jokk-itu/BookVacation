using DocumentClient;
using Mediator;
using Raven.Client.Documents;
using TrackingService.Domain;

namespace TrackingService.Infrastructure.Requests.UpdateTracking;

public class UpdateTrackingCommandHandler : ICommandHandler<UpdateTrackingCommand, Tracking>
{
    private readonly IDocumentClient _client;

    public UpdateTrackingCommandHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Response<Tracking>> Handle(UpdateTrackingCommand command, CancellationToken cancellationToken)
    {
        var tracking =
            await _client.QueryAsync<Tracking>(async query => await query.Where(x => x.Id == command.TrackingNumber)
                .FirstOrDefaultAsync(cancellationToken));

        if (tracking is null)
        {
            tracking = new Tracking
            {
                Id = command.TrackingNumber,
                Statuses = new List<Status>()
            };
            await _client.StoreAsync(tracking, cancellationToken);
        }

        tracking.Statuses.Add(new Status
        {
            Result = command.Result,
            OccuredAt = command.OccuredAt
        });

        await _client.UpdateAsync(cancellationToken);

        return new Response<Tracking>(tracking);
    }
}