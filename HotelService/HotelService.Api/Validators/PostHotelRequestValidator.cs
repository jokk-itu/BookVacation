using FluentValidation;
using HotelService.Contracts.CreateHotel;

namespace HotelService.Api.Validators;

public class PostHotelRequestValidator : AbstractValidator<PostHotelRequest>
{
    public PostHotelRequestValidator()
    {
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.Rooms).GreaterThan((short)0);
    }
}