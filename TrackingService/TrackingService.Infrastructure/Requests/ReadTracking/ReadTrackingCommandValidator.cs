using FluentValidation;
using TrackingService.Infrastructure.Requests.ReadTracking;

namespace TrackingService.Infrastructure.Validators;

public class ReadTrackingCommandValidator : AbstractValidator<ReadTrackingCommand>
{
    public ReadTrackingCommandValidator()
    {
        RuleFor(x => x.TrackingNumber).NotEmpty();
    }
}