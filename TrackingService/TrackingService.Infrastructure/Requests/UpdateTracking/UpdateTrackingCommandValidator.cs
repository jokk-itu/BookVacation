using FluentValidation;

namespace TrackingService.Infrastructure.Requests.UpdateTracking;

public class UpdateTrackingCommandValidator : AbstractValidator<UpdateTrackingCommand>
{
    public UpdateTrackingCommandValidator()
    {
        RuleFor(x => x.TrackingNumber).NotEmpty();
        RuleFor(x => x.OccuredAt).NotEmpty();
        RuleFor(x => x.Result).NotEmpty();
    }
}