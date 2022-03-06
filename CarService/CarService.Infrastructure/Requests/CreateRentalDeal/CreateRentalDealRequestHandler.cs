using CarService.Domain;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace CarService.Infrastructure.Requests.CreateRentalDeal;

public class CreateRentalDealRequestHandler : IRequestHandler<CreateRentalDealRequest, RentalDeal?>
{
    private readonly IAsyncDocumentSession _session;

    public CreateRentalDealRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<RentalDeal?> Handle(CreateRentalDealRequest request, CancellationToken cancellationToken)
    {
        var conflictingRentDeal = await _session.Query<RentalDeal>()
            .Where(x => x.RentalCarId.Equals(request.RentalCarId))
            .Where(x => (request.RentFrom >= x.RentFrom && request.RentFrom <= x.RentTo) || (request.RentTo >= x.RentFrom && request.RentTo <= x.RentTo))
            .FirstOrDefaultAsync(cancellationToken);

        if (conflictingRentDeal is not null)
            return null;

        var rentalDeal = new RentalDeal
        {
            RentFrom = request.RentFrom,
            RentTo = request.RentTo,
            RentalCarId = request.RentalCarId
        };

        await _session.StoreAsync(rentalDeal, cancellationToken);
        await _session.SaveChangesAsync(cancellationToken);
        return rentalDeal;
    }
}