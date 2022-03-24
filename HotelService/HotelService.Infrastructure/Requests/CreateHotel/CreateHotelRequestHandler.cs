using HotelService.Domain;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace HotelService.Infrastructure.Requests.CreateHotel;

public class CreateHotelRequestHandler : IRequestHandler<CreateHotelRequest, Hotel?>
{
    private readonly IAsyncDocumentSession _session;

    public CreateHotelRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<Hotel?> Handle(CreateHotelRequest request, CancellationToken cancellationToken)
    {
        var conflictingHotel = await _session.Query<Hotel>()
            .Where(x => x.Address == request.Address && x.City == request.City && x.Country == request.Country)
            .FirstOrDefaultAsync(cancellationToken);

        if (conflictingHotel is not null)
            return null;

        var hotel = new Hotel
        {
            HotelRooms = Enumerable.Range(0, request.Rooms).Select(_ => new HotelRoom {Id = Guid.NewGuid().ToString()}).ToList(),
            Country = request.Country,
            City = request.City,
            Address = request.Address
        };
        await _session.StoreAsync(hotel, cancellationToken);
        await _session.SaveChangesAsync(cancellationToken);
        return hotel;
    }
}