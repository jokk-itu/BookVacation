using FluentValidation;

namespace TrackingService.Infrastructure.Requests.ReadTracking;

public class ReadTrackingCommandValidator : AbstractValidator<ReadTrackingCommand>
{
    public ReadTrackingCommandValidator()
    {
        RuleFor(x => x.TrackingNumber).NotEmpty();
    }
}