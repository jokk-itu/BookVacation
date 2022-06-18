using CarService.Infrastructure.Requests.CreateRentalCar;
using FluentValidation;

namespace CarService.Infrastructure.Validators;

public class CreateRentalCarCommandValidator : AbstractValidator<CreateRentalCarCommand>
{
    public CreateRentalCarCommandValidator()
    {
        RuleFor(x => x.CarModelNumber).NotEmpty();
        RuleFor(x => x.Color).NotEmpty();
        RuleFor(x => x.CarCompanyName).NotEmpty();
        RuleFor(x => x.RentingCompanyName).NotEmpty();
        RuleFor(x => x.DayPrice).GreaterThan(0);
    }
}