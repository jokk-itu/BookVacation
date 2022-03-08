using FlightService.Contracts.Airplane;
using FlightService.Infrastructure.Requests.CreateAirplane;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlightService.Api.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AirplaneController : ControllerBase
{
    private readonly IMediator _mediator;

    public AirplaneController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostAirplaneRequest), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync(PostAirplaneRequest request, CancellationToken cancellationToken = default)
    {
        var airPlane = await _mediator.Send(new CreateAirplaneRequest(request.ModelNumber, request.AirplaneMakerName, request.AirlineName, request.Seats), cancellationToken);
        return Created("", new PostAirplaneResponse
        {
            Id = airPlane.Id,
            AirlineName = airPlane.AirlineName,
            AirplaneMakerName = airPlane.AirplaneMakerName,
            ModelNumber = airPlane.ModelNumber,
            Seats = airPlane.Seats.Select(s => new SeatDTO {Id = s.Id})
        });
    }
}