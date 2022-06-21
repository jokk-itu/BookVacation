using FluentValidation;

namespace FlightService.Infrastructure.Requests.CreateAirplane;

public class CreateAirplaneCommandValidator : AbstractValidator<CreateAirplaneCommand>
{
    public CreateAirplaneCommandValidator()
    {
        RuleFor(x => x.Seats).GreaterThan((short)0);
        RuleFor(x => x.AirlineName).NotEmpty();
        RuleFor(x => x.ModelNumber).NotEmpty();
        RuleFor(x => x.AirplaneMakerName).NotEmpty();
    }
}