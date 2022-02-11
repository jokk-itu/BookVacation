using System.ComponentModel.DataAnnotations.Schema;
using Contracts.BookFlightStateMachine;
using FlightService.Contracts.DTO;
using FlightService.Contracts.GetFlight;
using FlightService.Contracts.GetFlights;
using FlightService.Contracts.PostFlight;
using FlightService.Requests.CreateFlight;
using FlightService.Requests.ReadFlight;
using FlightService.Requests.ReadFlights;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlightService.Controllers;

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
    public async Task<IActionResult> GetFlightAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = new ReadFlightRequest(id);
        var (result, flight) = await _mediator.Send(request, cancellationToken);
        return result switch
        {
            Requests.Response.NotFound => NotFound(),
            Requests.Response.Ok => Ok(new GetFlightResponse { Id = flight.Id, From = flight.From, To = flight.To }),
            Requests.Response.BadRequest => BadRequest()
        };
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetFlightsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFlightsAsync(
        GetFlightsRequest request,
        CancellationToken cancellationToken = default)
    {
        var (result, flights) =
            await _mediator.Send(new ReadFlightsRequest(request.Amount, request.Offset), cancellationToken);
        return result switch
        {
            Requests.Response.Ok => Ok(flights.Select(flight => new Flight(flight.Id, flight.From, flight.To))),
            Requests.Response.BadRequest => BadRequest()
        };
    }

    [HttpPost]
    public async Task<IActionResult> PostFlightAsync(
        [FromBody] PostFlightRequest request,
        CancellationToken cancellationToken = default)
    {
        var (result, flight) = await _mediator.Send(new CreateFlightRequest(request.From, request.To), cancellationToken);
        return result switch
        {
            Requests.Response.Created => Created($"http://localhost:5001/api/v1/{flight.Id}", new Flight(flight.Id, flight.From, flight.To)),
            Requests.Response.Conflict => Conflict(),
            Requests.Response.BadRequest => BadRequest()
        };
    }
}