using CarService.Infrastructure.Requests.DeleteRentalDeal;
using FluentValidation;

namespace CarService.Infrastructure.Validators;

public class DeleteRentalDealRequestValidator : AbstractValidator<DeleteRentalDealRequest>
{
    public DeleteRentalDealRequestValidator()
    {
        RuleFor(x => x.RentalDealId).NotEmpty();
    }
}