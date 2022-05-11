using FlightService.Infrastructure.Requests.CreateFlightReservation;
using FluentValidation;

namespace FlightService.Infrastructure.Validators;

public class CreateFlightReservationRequestValidator : AbstractValidator<CreateFlightReservationRequest>
{
    public CreateFlightReservationRequestValidator()
    {
        RuleFor(x => x.FlightId).NotEmpty();
        RuleFor(x => x.SeatId).NotEmpty();
    }
}