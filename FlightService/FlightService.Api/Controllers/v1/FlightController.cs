using FlightService.Contracts.Flight;
using FlightService.Infrastructure.Requests.CreateFlight;
using Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlightService.Api.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FlightController : ControllerBase
{
    private readonly IMediator _mediator;

    public FlightController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostFlightResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync(PostFlightRequest request, CancellationToken cancellationToken = default)
    {
        var response =
            await _mediator.Send(
                new CreateFlightCommand(request.From, request.To, request.FromAirport, request.ToAirport,
                    request.AirplaneId, request.Price), cancellationToken);

        return response.ResponseCode switch
        {
            ResponseCode.NotFound => NotFound(),
            ResponseCode.Conflict => Conflict(),
            ResponseCode.Ok => Created("",
                new PostFlightResponse
                {
                    Id = Guid.Parse(response.Body!.Id),
                    From = response.Body!.From,
                    To = response.Body!.To,
                    FromAirport = response.Body!.FromAirport,
                    ToAirport = response.Body!.ToAirport,
                    AirPlaneId = response.Body!.AirPlaneId,
                    Price = response.Body!.Price
                })
        };
    }
}