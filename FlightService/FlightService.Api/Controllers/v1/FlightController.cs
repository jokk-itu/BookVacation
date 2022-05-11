using FlightService.Contracts.Flight;
using FlightService.Infrastructure.Requests.CreateFlight;
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
        var flight =
            await _mediator.Send(
                new CreateFlightRequest(request.From, request.To, request.FromAirport, request.ToAirport,
                    request.AirplaneId, request.Price), cancellationToken);

        if (flight is null)
            return Conflict();

        return Created("", new PostFlightResponse
        {
            Id = Guid.Parse(flight.Id),
            From = flight.From,
            To = flight.To,
            FromAirport = flight.FromAirport,
            ToAirport = flight.ToAirport,
            AirPlaneId = flight.AirPlaneId,
            Price = flight.Price
        });
    }
}