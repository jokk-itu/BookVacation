using MediatR;

namespace HotelService.Infrastructure.Requests.DeleteRentHotel;

public record DeleteRentHotelRequest(Guid RentId) : IRequest<RequestResult>;