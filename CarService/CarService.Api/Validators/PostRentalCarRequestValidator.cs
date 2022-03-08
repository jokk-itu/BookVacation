using CarService.Contracts.RentalCar;
using FluentValidation;

namespace CarService.Api.Validators;

public class PostRentalCarRequestValidator : AbstractValidator<PostRentalCarRequest>
{
    public PostRentalCarRequestValidator()
    {
        RuleFor(x => x.CarModelNumber).NotEmpty();
        RuleFor(x => x.Color).NotEmpty();
        RuleFor(x => x.CarCompanyName).NotEmpty();
        RuleFor(x => x.RentingCompanyName).NotEmpty();
        RuleFor(x => x.DayPrice).GreaterThan(0);
    }
}