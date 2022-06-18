using CarService.Infrastructure.Requests.DeleteRentalDeal;
using FluentValidation;

namespace CarService.Infrastructure.Validators;

public class DeleteRentalDealCommandValidator : AbstractValidator<DeleteRentalDealCommand>
{
    public DeleteRentalDealCommandValidator()
    {
        RuleFor(x => x.RentalDealId).NotEmpty();
    }
}