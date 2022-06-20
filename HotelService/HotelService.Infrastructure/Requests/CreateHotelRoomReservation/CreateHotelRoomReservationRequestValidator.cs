using FluentValidation;
using HotelService.Infrastructure.Requests.CreateHotelRoomReservation;

namespace HotelService.Infrastructure.Validators;

public class CreateHotelRoomReservationRequestValidator : AbstractValidator<CreateHotelRoomReservationCommand>
{
    public CreateHotelRoomReservationRequestValidator()
    {
        RuleFor(x => x.HotelId).NotEmpty();
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.From).NotEmpty();
        RuleFor(x => x.To).NotEmpty();
    }
}