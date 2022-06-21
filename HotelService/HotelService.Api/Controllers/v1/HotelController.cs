using HotelService.Contracts.CreateHotel;
using HotelService.Infrastructure.Requests.CreateHotel;
using Mediator;
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
        var response = await _mediator.Send(
            new CreateHotelCommand(request.Rooms, request.Country, request.City, request.Address),
            cancellationToken);

        if (response.ResponseCode != ResponseCode.Ok) return Conflict();

        return Created("", new PostHotelResponse
        {
            Id = Guid.Parse(response.Body!.Id),
            HotelRooms = response.Body!.HotelRooms.Select(x => new HotelRoomDTO { Id = Guid.Parse(x.Id) }),
            Country = response.Body!.Country,
            City = response.Body!.City,
            Address = response.Body!.Address
        });
    }
}