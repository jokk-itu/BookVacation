using CarService.Domain;
using DocumentClient;
using Mediator;
using MediatR;

namespace CarService.Infrastructure.Requests.CreateRentalCar;

public class CreateRentalCarCommandHandler : ICommandHandler<CreateRentalCarCommand, RentalCar>
{
    private readonly IDocumentClient _client;

    public CreateRentalCarCommandHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Response<RentalCar>> Handle(CreateRentalCarCommand command, CancellationToken cancellationToken)
    {
        var rentalCar = new RentalCar
        {
            CarModelNumber = command.CarModelNumber,
            CarCompanyName = command.CarCompanyName,
            RentalCompanyName = command.RentingCompanyName,
            DayPrice = command.DayPrice,
            Color = command.Color
        };
        await _client.StoreAsync(rentalCar, cancellationToken);
        return new Response<RentalCar>(rentalCar);
    }
}