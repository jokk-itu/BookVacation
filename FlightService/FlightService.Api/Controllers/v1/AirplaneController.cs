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
    public async Task<IActionResult> PostAsync(PostAirplaneRequest request,
        CancellationToken cancellationToken = default)
    {
        var response =
            await _mediator.Send(
                new CreateAirplaneCommand(request.ModelNumber, request.AirplaneMakerName, request.AirlineName,
                    request.Seats), cancellationToken);
        return Created("", new PostAirplaneResponse
        {
            Id = Guid.Parse(response.Body!.Id),
            AirlineName = response.Body!.AirlineName,
            AirplaneMakerName = response.Body!.AirplaneMakerName,
            ModelNumber = response.Body!.ModelNumber,
            Seats = response.Body!.Seats.Select(s => new SeatDTO { Id = Guid.Parse(s.Id) })
        });
    }
}