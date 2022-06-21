using FluentValidation;

namespace FlightService.Infrastructure.Requests.CreateFlight;

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