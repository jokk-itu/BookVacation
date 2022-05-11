using FluentValidation;
using TrackingService.Infrastructure.Requests.ReadTracking;

namespace TrackingService.Infrastructure.Validators;

public class ReadTrackingRequestValidator : AbstractValidator<ReadTrackingRequest>
{
    public ReadTrackingRequestValidator()
    {
        RuleFor(x => x.TrackingNumber).NotEmpty();
    }
}