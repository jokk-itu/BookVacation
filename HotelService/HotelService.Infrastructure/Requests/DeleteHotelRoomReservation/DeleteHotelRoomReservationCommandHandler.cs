using DocumentClient;
using Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;

public class DeleteHotelRoomReservationCommandHandler : ICommandHandler<DeleteHotelRoomReservationCommand, Unit>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<DeleteHotelRoomReservationCommandHandler> _logger;

    public DeleteHotelRoomReservationCommandHandler(IDocumentClient client, ILogger<DeleteHotelRoomReservationCommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<Unit>> Handle(DeleteHotelRoomReservationCommand command, CancellationToken cancellationToken)
    {
        var isDeleted = await _client.DeleteAsync(command.HotelRoomReservationId.ToString(), cancellationToken);

        if (isDeleted)
            return new Response<Unit>();
        
        _logger.LogError("HotelRoomReservation with identifier {Identifier} does not exist", command.HotelRoomReservationId);
        return new Response<Unit>(ResponseCode.NotFound, new []{ "HotelRoomReservation does not exist" });
    }
}