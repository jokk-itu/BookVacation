using MediatR;

namespace HotelService.Infrastructure.Requests.DeleteBookHotel;

public record DeleteBookHotelRequest(Guid RentId) : IRequest<RequestResult>;