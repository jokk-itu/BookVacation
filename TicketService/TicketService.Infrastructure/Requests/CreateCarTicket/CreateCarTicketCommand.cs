using Mediator;
using MediatR;

namespace TicketService.Infrastructure.Requests.CreateCarTicket;

public record CreateCarTicketCommand(Guid CarId, string RentingCompanyName) : ICommand<Unit>;