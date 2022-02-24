using MediatR;

namespace HotelService.Infrastructure.Requests.CreateBookHotel;

public record CreateBookHotelRequest(Guid HotelId, Guid RoomId, uint Days, Guid RentId) : IRequest<RequestResult>
{
}