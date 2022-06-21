using DocumentClient;
using Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using TrackingService.Domain;

namespace TrackingService.Infrastructure.Requests.ReadTracking;

public class ReadTrackingCommandHandler : ICommandHandler<ReadTrackingCommand, Tracking>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<ReadTrackingCommandHandler> _logger;

    public ReadTrackingCommandHandler(IDocumentClient client, ILogger<ReadTrackingCommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<Tracking>> Handle(ReadTrackingCommand command, CancellationToken cancellationToken)
    {
        var tracking = await _client.QueryAsync<Tracking>(async query => await query
            .Where(x => x.Id == command.TrackingNumber)
            .FirstOrDefaultAsync(cancellationToken));

        if (tracking is not null) 
            return new Response<Tracking>(tracking);
        
        _logger.LogDebug("Tracking with identifier {} does not exist", command.TrackingNumber);
        return new Response<Tracking>(ResponseCode.NotFound, new []{ "Tracking does not exist" });

    }
}