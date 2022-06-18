using CarService.Domain;
using Mediator;

namespace CarService.Infrastructure.Requests.CreateRentalDeal;

public record CreateRentalDealCommand
    (DateTimeOffset RentFrom, DateTimeOffset RentTo, Guid RentalCarId) : ICommand<RentalDeal>;