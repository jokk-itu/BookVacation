using MediatR;

namespace HotelService.Requests.CreateRentHotel;

public record CreateRentHotelRequest(Guid HotelId, Guid RoomId, uint Days, Guid RentId) : IRequest<RequestResult>
{
}