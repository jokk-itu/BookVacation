using FlightService.Contracts.Airplane;
using FluentValidation;

namespace FlightService.Api.Validators;

public class PostAirplaneRequestValidator : AbstractValidator<PostAirplaneRequest>
{
    public PostAirplaneRequestValidator()
    {
        RuleFor(x => x.Seats).GreaterThan((short)0);
        RuleFor(x => x.AirlineName).NotEmpty();
        RuleFor(x => x.ModelNumber).NotEmpty();
        RuleFor(x => x.AirplaneMakerName).NotEmpty();
    }
}