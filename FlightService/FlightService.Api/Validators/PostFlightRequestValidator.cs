using FlightService.Contracts.Flight;
using FluentValidation;

namespace FlightService.Api.Validators;

public class PostFlightRequestValidator : AbstractValidator<PostFlightRequest>
{
    public PostFlightRequestValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.AirPlaneId).NotEmpty();
        RuleFor(x => x.FromAirport).NotEmpty();
        RuleFor(x => x.ToAirport).NotEmpty();
        RuleFor(x => x.To).GreaterThan(x => x.From);
    }
}