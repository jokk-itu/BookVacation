using CarService.Domain;
using DocumentClient;
using Mediator;
using MediatR;

namespace CarService.Infrastructure.Requests.CreateRentalCar;

public class CreateRentalCarRequestHandler : ICommandHandler<CreateRentalCarRequest, RentalCar>
{
    private readonly IDocumentClient _client;

    public CreateRentalCarRequestHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Response<RentalCar>> Handle(CreateRentalCarRequest request, CancellationToken cancellationToken)
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
        return new Response<RentalCar>(rentalCar);
    }
}