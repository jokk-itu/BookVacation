using FluentValidation;
using HotelService.Infrastructure.Requests.CreateHotel;

namespace HotelService.Infrastructure.Validators;

public class CreateHotelRequestValidator : AbstractValidator<CreateHotelCommand>
{
    public CreateHotelRequestValidator()
    {
        RuleFor(x => x.Rooms).GreaterThan((short)0);
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}