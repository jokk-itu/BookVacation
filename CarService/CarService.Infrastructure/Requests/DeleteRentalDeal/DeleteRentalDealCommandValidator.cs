using FluentValidation;

namespace CarService.Infrastructure.Requests.DeleteRentalDeal;

public class DeleteRentalDealCommandValidator : AbstractValidator<DeleteRentalDealCommand>
{
    public DeleteRentalDealCommandValidator()
    {
        RuleFor(x => x.RentalDealId).NotEmpty();
    }
}