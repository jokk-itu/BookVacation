using FluentValidation;

namespace HotelService.Infrastructure.Requests.CreateHotelRoomReservation;

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