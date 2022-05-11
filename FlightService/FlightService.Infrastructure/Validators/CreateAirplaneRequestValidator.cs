using FlightService.Infrastructure.Requests.CreateAirplane;
using FluentValidation;

namespace FlightService.Infrastructure.Validators;

public class CreateAirplaneRequestValidator : AbstractValidator<CreateAirplaneRequest>
{
    public CreateAirplaneRequestValidator()
    {
        RuleFor(x => x.Seats).GreaterThan((short)0);
        RuleFor(x => x.AirlineName).NotEmpty();
        RuleFor(x => x.ModelNumber).NotEmpty();
        RuleFor(x => x.AirplaneMakerName).NotEmpty();
    }
}