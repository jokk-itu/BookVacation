using DocumentClient;
using HotelService.Domain;
using Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;

namespace HotelService.Infrastructure.Requests.CreateHotel;

public class CreateHotelCommandHandler : ICommandHandler<CreateHotelCommand, Hotel>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<CreateHotelCommandHandler> _logger;

    public CreateHotelCommandHandler(IDocumentClient client, ILogger<CreateHotelCommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<Hotel>> Handle(CreateHotelCommand command, CancellationToken cancellationToken)
    {
        var conflictingHotel = await _client.QueryAsync<Hotel>(async query => await query
            .Where(x => x.Address == command.Address && x.City == command.City && x.Country == command.Country)
            .FirstOrDefaultAsync(cancellationToken));

        if (conflictingHotel is not null)
        {
            _logger.LogDebug("Hotel on address {} in city {} in country {} already exists",
                command.Address, command.City, command.Country);
            return new Response<Hotel>(ResponseCode.Conflict, new []{ "Hotel already exists" });
        }

        var hotel = new Hotel
        {
            HotelRooms = Enumerable.Range(0, command.Rooms)
                .Select(_ => new HotelRoom { Id = Guid.NewGuid().ToString() }).ToList(),
            Country = command.Country,
            City = command.City,
            Address = command.Address
        };
        await _client.StoreAsync(hotel, cancellationToken);
        return new Response<Hotel>(hotel);
    }
}