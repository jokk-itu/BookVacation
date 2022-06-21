using Mediator;
using MediatR;

namespace TicketService.Infrastructure.Requests.CreateFlightTicket;

public record CreateFlightTicketCommand(Guid FlightId) : ICommand<Unit>;