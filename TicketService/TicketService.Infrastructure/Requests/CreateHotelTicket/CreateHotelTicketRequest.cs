using MediatR;

namespace TicketService.Infrastructure.Requests.CreateHotelTicket;

public record CreateHotelTicketRequest(Guid HotelId, Guid RoomId) : IRequest<RequestResult>;