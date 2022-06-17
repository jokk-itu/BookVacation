using Mediator;
using MediatR;

namespace CarService.Infrastructure.Requests.DeleteRentalDeal;

public record DeleteRentalDealRequest(Guid RentalDealId) : ICommand<Unit>;