using HotelService.Domain;
using Mediator;
using MediatR;

namespace HotelService.Infrastructure.Requests.CreateHotel;

public record CreateHotelCommand(short Rooms, string Country, string City, string Address) : ICommand<Hotel>;