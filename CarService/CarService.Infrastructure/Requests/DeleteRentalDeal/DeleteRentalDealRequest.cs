using MediatR;

namespace CarService.Infrastructure.Requests.DeleteRentalDeal;

public record DeleteRentalDealRequest(string RentalDealId) : IRequest<Unit>;