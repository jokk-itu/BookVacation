using FlightService.Contracts.DTO;
using FlightService.Contracts.GetFlight;
using FlightService.Contracts.GetFlights;
using FlightService.Contracts.PostFlight;
using FlightService.Infrastructure.Requests;
using FlightService.Infrastructure.Requests.CreateFlight;
using FlightService.Infrastructure.Requests.ReadFlight;
using FlightService.Infrastructure.Requests.ReadFlights;
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

    [HttpGet]
    [Route("{id:guid}")]
    [ProducesResponseType(typeof(GetFlightResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFlightAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var request = new ReadFlightRequest(id);
        var (result, flight) = await _mediator.Send(request, cancellationToken);
        return result switch
        {
            RequestResult.NotFound => NotFound(),
            RequestResult.Ok => Ok(new GetFlightResponse { Id = flight!.Id, From = flight.From, To = flight.To }),
            _ => throw new NotSupportedException()
        };
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetFlightsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFlightsAsync(
        [FromQuery] GetFlightsRequest request,
        CancellationToken cancellationToken = default)
    {
        var (result, flights) =
            await _mediator.Send(new ReadFlightsRequest(request.Amount, request.Offset), cancellationToken);
        return result switch
        {
            RequestResult.Ok => Ok(flights!.Select(flight => new Flight(flight.Id, flight.From, flight.To))),
            RequestResult.NotFound => NotFound(),
            _ => throw new NotSupportedException()
        };
    }

    [HttpPost]
    [ProducesResponseType(typeof(Flight), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostFlightAsync(
        [FromBody] PostFlightRequest request,
        CancellationToken cancellationToken = default)
    {
        var (result, flight) =
            await _mediator.Send(new CreateFlightRequest(request.From, request.To), cancellationToken);
        return result switch
        {
            RequestResult.Created => Created($"http://localhost:5001/api/v1/{flight!.Id}",
                new Flight(flight.Id, flight.From, flight.To)),
            RequestResult.Error => BadRequest(),
            _ => throw new NotSupportedException("")
        };
    }
}