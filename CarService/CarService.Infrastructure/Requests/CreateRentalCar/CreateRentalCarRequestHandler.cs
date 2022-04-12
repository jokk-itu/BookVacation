using CarService.Domain;
using DocumentClient;
using MediatR;

namespace CarService.Infrastructure.Requests.CreateRentalCar;

public class CreateRentalCarRequestHandler : IRequestHandler<CreateRentalCarRequest, RentalCar>
{
    private readonly IDocumentClient _client;

    public CreateRentalCarRequestHandler(IDocumentClient client)
    {
        _client = client;
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
        await _client.StoreAsync(rentalCar, cancellationToken);
        return rentalCar;
    }
}