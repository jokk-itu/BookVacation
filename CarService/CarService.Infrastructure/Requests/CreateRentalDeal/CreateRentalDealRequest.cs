using CarService.Domain;
using MediatR;

namespace CarService.Infrastructure.Requests.CreateRentalDeal;

public record CreateRentalDealRequest
    (DateTimeOffset RentFrom, DateTimeOffset RentTo, string RentalCarId) : IRequest<RentalDeal?>;