using FluentValidation;
using TrackingService.Infrastructure.Requests.UpdateTracking;

namespace TrackingService.Infrastructure.Validators;

public class UpdateTrackingCommandValidator : AbstractValidator<UpdateTrackingCommand>
{
    public UpdateTrackingCommandValidator()
    {
        RuleFor(x => x.TrackingNumber).NotEmpty();
        RuleFor(x => x.OccuredAt).NotEmpty();
        RuleFor(x => x.Result).NotEmpty();
    }
}