using FluentValidation;
using TrackingService.Infrastructure.Requests.UpdateTracking;

namespace TrackingService.Infrastructure.Validators;

public class UpdateTrackingRequestValidator : AbstractValidator<UpdateTrackingRequest>
{
    public UpdateTrackingRequestValidator()
    {
        RuleFor(x => x.TrackingNumber).NotEmpty();
        RuleFor(x => x.OccuredAt).NotEmpty();
        RuleFor(x => x.Result).NotEmpty();
    }
}