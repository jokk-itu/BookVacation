using FlightService.Contracts.GetFlights;
using FluentValidation;

namespace FlightService.Api.Validators;

public class GetFlightsRequestValidator : AbstractValidator<GetFlightsRequest>
{
    public GetFlightsRequestValidator()
    {
        RuleFor(x => x.Amount)
            .InclusiveBetween(1u, 200u)
            .WithMessage("Amount must be greater than 1 and not exceed 200");
    }
}