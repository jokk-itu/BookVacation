using FluentValidation;

namespace FlightService.Infrastructure.Requests.CreateFlightReservation;

public class CreateFlightReservationCommandValidator : AbstractValidator<CreateFlightReservationCommand>
{
    public CreateFlightReservationCommandValidator()
    {
        RuleFor(x => x.FlightId).NotEmpty();
        RuleFor(x => x.SeatId).NotEmpty();
    }
}