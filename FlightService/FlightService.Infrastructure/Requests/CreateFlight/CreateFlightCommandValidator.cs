using FlightService.Infrastructure.Requests.CreateFlight;
using FluentValidation;

namespace FlightService.Infrastructure.Validators;

public class CreateFlightCommandValidator : AbstractValidator<CreateFlightCommand>
{
    public CreateFlightCommandValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.AirplaneId).NotEmpty();
        RuleFor(x => x.FromAirport).NotEmpty();
        RuleFor(x => x.ToAirport).NotEmpty();
        RuleFor(x => x.To).GreaterThan(x => x.From);
    }
}