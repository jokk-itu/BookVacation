using FlightService.Infrastructure.Requests.CreateFlightReservation;
using FluentValidation;

namespace FlightService.Infrastructure.Validators;

public class CreateFlightReservationCommandValidator : AbstractValidator<CreateFlightReservationCommand>
{
    public CreateFlightReservationCommandValidator()
    {
        RuleFor(x => x.FlightId).NotEmpty();
        RuleFor(x => x.SeatId).NotEmpty();
    }
}