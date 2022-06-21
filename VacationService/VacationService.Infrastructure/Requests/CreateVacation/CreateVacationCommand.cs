using Mediator;
using MediatR;

namespace VacationService.Infrastructure.Requests.CreateVacation;

public record CreateVacationCommand(Guid FlightId, Guid FlightSeatId, Guid HotelId, DateTimeOffset HotelFrom, DateTimeOffset HotelTo, Guid HotelRoomId, Guid RentalCarId, string RentingCompanyName, DateTimeOffset RentalCarFrom, DateTimeOffset RentalCarTo) : ICommand<Unit>;