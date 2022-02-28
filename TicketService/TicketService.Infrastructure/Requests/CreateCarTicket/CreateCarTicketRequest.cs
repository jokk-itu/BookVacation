using MediatR;

namespace TicketService.Infrastructure.Requests.CreateCarTicket;

public record CreateCarTicketRequest(Guid CarId, Guid RentingCompanyId) : IRequest<RequestResult>;