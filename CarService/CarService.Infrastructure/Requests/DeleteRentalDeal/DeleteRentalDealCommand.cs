using Mediator;
using MediatR;

namespace CarService.Infrastructure.Requests.DeleteRentalDeal;

public record DeleteRentalDealCommand(Guid RentalDealId) : ICommand<Unit>;