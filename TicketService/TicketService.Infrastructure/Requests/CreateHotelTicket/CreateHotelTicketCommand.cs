using Mediator;
using MediatR;

namespace TicketService.Infrastructure.Requests.CreateHotelTicket;

public record CreateHotelTicketCommand(Guid HotelId, Guid RoomId) : ICommand<Unit>;