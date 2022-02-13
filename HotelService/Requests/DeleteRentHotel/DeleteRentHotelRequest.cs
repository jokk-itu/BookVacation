using MediatR;

namespace HotelService.Requests.DeleteRentHotel;

public record DeleteRentHotelRequest(Guid RentId) : IRequest<RequestResult>;