using FluentValidation;

namespace CarService.Infrastructure.Requests.CreateRentalDeal;

public class CreateRentalDealCommandValidator : AbstractValidator<CreateRentalDealCommand>
{
    public CreateRentalDealCommandValidator()
    {
        RuleFor(x => x.RentalCarId).NotEmpty();
        RuleFor(x => x.RentFrom).NotNull();
        RuleFor(x => x.RentTo).NotNull();
        RuleFor(x => x.RentFrom).LessThan(x => x.RentTo);
    }
}