using MediatR;

namespace HotelService.Infrastructure.Requests.CreateRentHotel;

public record CreateRentHotelRequest(Guid HotelId, Guid RoomId, uint Days, Guid RentId) : IRequest<RequestResult>
{
}