using CarService.Domain;
using MediatR;

namespace CarService.Infrastructure.Requests.CreateRentalCar;

public record CreateRentalCarRequest(Guid CarModelNumber, string CarCompanyName, string RentingCompanyName, decimal DayPrice, string Color) : IRequest<RentalCar>;