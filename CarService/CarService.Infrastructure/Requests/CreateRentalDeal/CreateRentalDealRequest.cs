using CarService.Domain;
using Mediator;

namespace CarService.Infrastructure.Requests.CreateRentalDeal;

public record CreateRentalDealRequest
    (DateTimeOffset RentFrom, DateTimeOffset RentTo, Guid RentalCarId) : ICommand<RentalDeal>;