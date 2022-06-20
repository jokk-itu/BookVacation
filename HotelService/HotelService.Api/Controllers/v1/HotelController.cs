using HotelService.Contracts.CreateHotel;
using HotelService.Infrastructure.Requests.CreateHotel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelService.Api.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class HotelController : ControllerBase
{
    private readonly IMediator _mediator;

    public HotelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostHotelResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PostHotelAsync(PostHotelRequest request, CancellationToken cancellationToken)
    {
        var hotel = await _mediator.Send(
            new CreateHotelCommand(request.Rooms, request.Country, request.City, request.Address),
            cancellationToken);

        if (hotel is null)
            return Conflict();

        return Created("", new PostHotelResponse
        {
            Id = Guid.Parse(hotel.Id),
            HotelRooms = hotel.HotelRooms.Select(x => new HotelRoomDTO { Id = Guid.Parse(x.Id) }),
            Country = hotel.Country,
            City = hotel.City,
            Address = hotel.Address
        });
    }
}