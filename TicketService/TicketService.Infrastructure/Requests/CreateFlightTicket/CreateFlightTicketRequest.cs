using MediatR;

namespace TicketService.Infrastructure.Requests.CreateFlightTicket;

public record CreateFlightTicketRequest(Guid FlightId) : IRequest<RequestResult>;