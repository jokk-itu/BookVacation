using FlightService.Contracts.PostFlight;
using FluentValidation;

namespace FlightService.Validators;

public class PostFlightRequestValidator : AbstractValidator<PostFlightRequest>
{
    public PostFlightRequestValidator()
    {
        RuleFor(x => x.From).GreaterThan(DateTime.Now);
        RuleFor(x => x.To).GreaterThan(DateTime.Now);
        RuleFor(x => x.From).LessThan(x => x.To);
    }
}