using MediatR;

namespace TicketService.Infrastructure.Requests.CreateCarTicket;

public record CreateCarTicketRequest(Guid CarId, string RentingCompanyName) : IRequest<RequestResult>;