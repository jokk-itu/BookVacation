using CarService.Domain;
using Mediator;
using MediatR;

namespace CarService.Infrastructure.Requests.CreateRentalCar;

public record CreateRentalCarCommand(Guid CarModelNumber, string CarCompanyName, string RentingCompanyName,
    decimal DayPrice, string Color) : ICommand<RentalCar>;