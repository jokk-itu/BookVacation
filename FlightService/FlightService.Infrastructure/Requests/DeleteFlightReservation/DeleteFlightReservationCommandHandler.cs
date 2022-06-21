using DocumentClient;
using Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightService.Infrastructure.Requests.DeleteFlightReservation;

public class DeleteFlightReservationCommandHandler : ICommandHandler<DeleteFlightReservationCommand, Unit>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<DeleteFlightReservationCommandHandler> _logger;

    public DeleteFlightReservationCommandHandler(IDocumentClient client, ILogger<DeleteFlightReservationCommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<Unit>> Handle(DeleteFlightReservationCommand command, CancellationToken cancellationToken)
    {
        var isDeleted = await _client.DeleteAsync(command.ReservationId.ToString(), cancellationToken);

        if (isDeleted)
            return new Response<Unit>();
        
        _logger.LogError("FlightReservation with identifier {} does not exist", command.ReservationId);
        return new Response<Unit>(ResponseCode.NotFound, new []{ "FlightReservation does not exist" });
    }
}