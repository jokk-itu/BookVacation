using DocumentClient;
using FlightService.Domain;
using Mediator;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;

namespace FlightService.Infrastructure.Requests.CreateFlightReservation;

public class CreateFlightReservationCommandHandler : ICommandHandler<CreateFlightReservationCommand, FlightReservation>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<CreateFlightReservationCommandHandler> _logger;

    public CreateFlightReservationCommandHandler(IDocumentClient client, ILogger<CreateFlightReservationCommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<FlightReservation>> Handle(CreateFlightReservationCommand request,
        CancellationToken cancellationToken)
    {
        var flight = await _client.QueryAsync<Flight>(query => query.Where(x => x.Id == request.FlightId.ToString())
            .FirstOrDefaultAsync(cancellationToken));

        if (flight is null)
        {
            _logger.LogDebug("Flight with identifier {Identifier} does not exist", request.FlightId);
            return new Response<FlightReservation>(ResponseCode.NotFound, new []{ "Flight does not exist" });
        }

        var conflictingReservation = await _client.QueryAsync<FlightReservation>(query => query
            .Where(x => x.FlightId == request.FlightId && x.SeatId == request.SeatId)
            .FirstOrDefaultAsync(cancellationToken));

        if (conflictingReservation is not null)
        {
            _logger.LogDebug("Seat with identifier {Identifier} is already booked", request.SeatId);
            return new Response<FlightReservation>(ResponseCode.Conflict, new []{ "Seat is already booked" });
        }

        var flightReservation = new FlightReservation
        {
            SeatId = request.SeatId,
            FlightId = request.FlightId
        };
        await _client.StoreAsync(flightReservation, cancellationToken);
        return new Response<FlightReservation>(flightReservation);
    }
}