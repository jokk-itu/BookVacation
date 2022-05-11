using CarService.Infrastructure.Requests.CreateRentalDeal;
using FluentValidation;

namespace CarService.Infrastructure.Validators;

public class CreateRentalDealRequestValidator : AbstractValidator<CreateRentalDealRequest>
{
    public CreateRentalDealRequestValidator()
    {
        RuleFor(x => x.RentalCarId).NotEmpty();
        RuleFor(x => x.RentFrom).NotNull();
        RuleFor(x => x.RentTo).NotNull();
        RuleFor(x => x.RentFrom).LessThan(x => x.RentTo);
    }
}