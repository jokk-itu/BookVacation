using FluentValidation;
using VacationService.Infrastructure.Requests.CreateVacation;

namespace VacationService.Infrastructure.Validators;

public class CreateVacationRequestValidator : AbstractValidator<CreateVacationRequest>
{
    public CreateVacationRequestValidator()
    {
        RuleFor(x => x.FlightId).NotEmpty();
        RuleFor(x => x.FlightSeatId).NotEmpty();
        RuleFor(x => x.HotelId).NotEmpty();
        RuleFor(x => x.HotelRoomId).NotEmpty();
        RuleFor(x => x.HotelFrom).LessThan(x => x.HotelTo);
        RuleFor(x => x.RentalCarId).NotEmpty();
        RuleFor(x => x.RentingCompanyName).NotEmpty();
        RuleFor(x => x.RentalCarFrom).LessThan(x => x.RentalCarTo);
    }
}