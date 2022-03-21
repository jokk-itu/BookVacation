using HotelService.Domain;
using MediatR;

namespace HotelService.Infrastructure.Requests.CreateHotel;

public record CreateHotelRequest(short Rooms, string Country, string City, string Address) : IRequest<Hotel?>;