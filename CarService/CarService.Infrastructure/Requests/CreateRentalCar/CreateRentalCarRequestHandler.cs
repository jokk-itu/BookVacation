using CarService.Domain;
using MediatR;
using Raven.Client.Documents.Session;

namespace CarService.Infrastructure.Requests.CreateRentalCar;

public class CreateRentalCarRequestHandler : IRequestHandler<CreateRentalCarRequest, RentalCar>
{
    private readonly IAsyncDocumentSession _session;

    public CreateRentalCarRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<RentalCar> Handle(CreateRentalCarRequest request, CancellationToken cancellationToken)
    {
        var rentalCar = new RentalCar
        {
            CarModelNumber = request.CarModelNumber,
            CarCompanyName = request.CarCompanyName,
            RentalCompanyName = request.RentingCompanyName,
            DayPrice = request.DayPrice,
            Color = request.Color
        };
        await _session.StoreAsync(rentalCar, cancellationToken);

        await _session.SaveChangesAsync(cancellationToken);
        return rentalCar;
    }
}